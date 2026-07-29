<#
.SYNOPSIS
    Simula um Arduino rodando o sketch SimpleDyno_Sketch.ino, enviando linhas CSV
    de 11 campos por uma porta serial (real ou virtual via com0com).

.DESCRIPTION
    Reproduz o formato exato esperado por Main.vb / DataReceivedHandler:
      [0] timestamp de sessao (micros(), microssegundos desde o inicio)
      [1] timestamp do ultimo pulso de RPM1 (micros())
      [2] intervalo entre os dois ultimos pulsos de RPM1 (microssegundos)
      [3] timestamp do ultimo pulso de RPM2 (sempre 0 aqui -- so simula 1 canal)
      [4] intervalo entre pulsos de RPM2 (sempre 0 aqui)
      [5] Voltage   (ADC 0-1023)
      [6] Current   (ADC 0-1023)
      [7] Temperature1 (ADC 0-1023)
      [8] Temperature2 (ADC 0-1023)
      [9] Pin4 Value (ADC 0-1023)
      [10] Pin5 Value (ADC 0-1023)

    RPM1 sobe de -StartRpm a -EndRpm ao longo de -DurationSeconds, simulando uma
    puxada real, assumindo Signals per RPM = 1 (padrao do SimpleDyno).

    NAO substitui o teste com o Arduino real: nao emula corrupcao eletrica de
    baud rate incompatilvel (com0com nao emula nivel de bit), so valida que o
    parsing/calibracao/plumbing do app funciona igual em cada baud rate.

.PARAMETER Port
    Porta serial para escrever (ex.: COM8, o lado "Arduino" de um par com0com).
    O SimpleDyno deve estar conectado na outra ponta do par (ex.: COM9).

.PARAMETER BaudRate
    Deve bater com o baud rate selecionado no combo do SimpleDyno.

.EXAMPLE
    .\Send-SimulatedDynoData.ps1 -Port COM8 -BaudRate 9600
    .\Send-SimulatedDynoData.ps1 -Port COM8 -BaudRate 115200 -DurationSeconds 12
#>
param(
    [Parameter(Mandatory = $true)][string]$Port,
    [int]$BaudRate = 9600,
    [double]$DurationSeconds = 8,
    [double]$StartRpm = 800,
    [double]$EndRpm = 6000
)

$ErrorActionPreference = "Stop"

$sp = New-Object System.IO.Ports.SerialPort $Port, $BaudRate, ([System.IO.Ports.Parity]::None), 8, ([System.IO.Ports.StopBits]::One)
$sp.Handshake = [System.IO.Ports.Handshake]::None
$sp.WriteTimeout = 1000

try {
    $sp.Open()
}
catch {
    Write-Error "Nao consegui abrir $Port em $BaudRate baud: $($_.Exception.Message)"
    exit 1
}

Write-Host "Enviando dados simulados em $Port a $BaudRate baud por $DurationSeconds s (RPM1 $StartRpm -> $EndRpm)."
Write-Host "Conecte o SimpleDyno na outra ponta do par serial. Ctrl+C para parar antes do fim."

$rng = New-Object System.Random
$sw = [System.Diagnostics.Stopwatch]::StartNew()
$lastPulseUs = 0.0
$pulseIntervalUs = 0.0

try {
    while ($sw.Elapsed.TotalSeconds -lt $DurationSeconds) {
        $tSec = $sw.Elapsed.TotalSeconds
        $frac = [Math]::Min(1.0, $tSec / $DurationSeconds)
        $eased = 1 - [Math]::Pow(1 - $frac, 2.1)
        $rpm = $StartRpm + ($EndRpm - $StartRpm) * $eased

        $nowUs = $tSec * 1000000.0
        $expectedIntervalUs = 60000000.0 / [Math]::Max($rpm, 1)

        if (($nowUs - $lastPulseUs) -ge $expectedIntervalUs) {
            $pulseIntervalUs = $nowUs - $lastPulseUs
            if ($lastPulseUs -eq 0) { $pulseIntervalUs = $expectedIntervalUs }
            $lastPulseUs = $nowUs
        }

        $volt  = 600 + $rng.Next(-5, 5)
        $curr  = [Math]::Min(1023, [int](200 + ($rpm / 6000.0) * 500) + $rng.Next(-5, 5))
        $temp1 = 420 + $rng.Next(-3, 3)
        $temp2 = 400 + $rng.Next(-3, 3)
        $pin4  = 512
        $pin5  = 512

        $line = "{0},{1},{2},0,0,{3},{4},{5},{6},{7},{8}" -f `
            [int64]$nowUs, [int64]$lastPulseUs, [int64]$pulseIntervalUs, `
            [int]$volt, [int]$curr, [int]$temp1, [int]$temp2, [int]$pin4, [int]$pin5

        # Arduino println() termina em CR+LF -- reproduz igual pra ser fiel ao hardware real.
        $sp.Write($line + "`r`n")
        Start-Sleep -Milliseconds 10
    }
}
finally {
    $sp.Close()
    Write-Host "Encerrado."
}
