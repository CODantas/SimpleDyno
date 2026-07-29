# Simulador de Arduino para teste do SimpleDyno

Ferramenta de apoio para validar a Fase 1 do plano de atualização (troca de WMI
por `SerialPort.GetPortNames()`) sem precisar reconectar o Arduino físico e
trocar o baud rate no sketch a cada teste.

**Isso não substitui o teste com o Arduino real.** Ele confirma que o app abre
a porta e faz o parsing/calibração dos 11 campos corretamente em cada baud
rate. Ele **não** emula corrupção elétrica de baud rate incompatível — esse
teste específico ("1 baud rate errado de propósito") só é válido com hardware
de verdade.

## Passo a passo

1. **Instale o com0com** (driver de par de portas seriais virtuais, gratuito e
   open-source para Windows). Procure por "com0com" — é o driver padrão da
   comunidade para isso.
2. Abra o "com0com Setup" e confirme que existe um par (por padrão
   `CNCA0 <-> CNCB0`). Renomeie para algo memorável, ex. `COM8 <-> COM9`, pelo
   próprio utilitário (`change PortName=COM8` / `change PortName=COM9`).
3. No SimpleDyno, abra o combo de portas COM e selecione `COM9` (o lado que
   ficará "livre" para o app).
4. No PowerShell (Windows, não precisa rodar como admin), execute o script
   apontando para `COM8` (o outro lado do par) no mesmo baud rate selecionado
   no app:

   ```powershell
   cd tools\arduino-simulator
   .\Send-SimulatedDynoData.ps1 -Port COM8 -BaudRate 9600
   ```

5. Conecta no SimpleDyno (`COM9`, mesmo baud, 9600 no exemplo). Os gauges de
   RPM1 devem subir suavemente de ~800 para ~6000 RPM ao longo de 8 segundos,
   e os valores de tensão/corrente/temperatura devem se mover (calibração
   aplicada).
6. Repete o passo 4/5 para os outros 6 baud rates, trocando `-BaudRate` no
   script e no combo do app: `14400, 19200, 28800, 38400, 57600, 115200`.

Se o app travar, não reconhecer a porta, mostrar `#ERROR#`/valores absurdos
nos gauges, ou lançar exceção em qualquer um desses passos, isso é uma
regressão da Fase 1 e precisa ser investigado antes de mergear.

## Parâmetros do script

| Parâmetro | Padrão | Descrição |
|---|---|---|
| `-Port` | (obrigatório) | Porta serial para escrever, ex. `COM8` |
| `-BaudRate` | `9600` | Tem que bater com o combo do SimpleDyno |
| `-DurationSeconds` | `8` | Duração da "puxada" simulada |
| `-StartRpm` / `-EndRpm` | `800` / `6000` | Faixa de RPM simulada |
