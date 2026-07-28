# SimpleDyno

Aplicativo Windows (VB.NET / WinForms, .NET Framework 4.8) para aquisição de dados de dinamômetro caseiro — RPM, torque, potência, drag, coastdown — via áudio (microfone) e/ou porta serial (Arduino), com gráficos e medidores em tempo real, curve-fitting (Levenberg-Marquardt) e exportação de dados.

Criado originalmente por Damian Cunningham ([DamoRC](https://github.com/DamoRC)). Este fork está em desenvolvimento ativo, adaptando o app para uso com hardware físico real (Arduino + sensores) de um dinamômetro em operação.

## Status

Não está arquivado — em desenvolvimento ativo. Só quem possui o hardware físico consegue validar mudanças de aquisição/calibração na bancada real; por isso o [simulador de Arduino](tools/arduino-simulator/) existe, para testar o fluxo serial sem o hardware.

## Principais recursos

- **Aquisição em tempo real** via áudio (placa de som) e/ou porta serial, com detecção de threshold configurável por canal.
- **Interface bilíngue** (Português/Inglês), troca em tempo de execução via botão dedicado (`My.Settings.Idioma`).
- **Tema escuro modernizado**: paleta de cores e tipografia próprias (`ColorPalette`/`TypographyManager`), três widgets de desenho GDI+ (medidor analógico, cartão digital, gráfico multi-eixo em tempo real) que substituem os widgets originais preservando 100% da lógica de aquisição/cálculo.
- **Curve-fitting** (Levenberg-Marquardt) para suavização de RPM e cálculo de torque/potência a partir da inércia do rolo.
- **Correção de coastdown** e configuração de parâmetros físicos do dyno (massa, diâmetros, relação de marcha).
- **Análise pós-coleta** com sobreposição de até 3 arquivos (OxyPlot/LiveCharts), incluindo exportação de relatório em PDF para o cliente (gráfico + tabela de picos + dados do cliente/veículo), sem depender de biblioteca de PDF externa (usa a impressora "Microsoft Print to PDF" do Windows).
- **Resiliência**: autosave periódico e recuperação de sessão em caso de crash durante coleta ativa.
- **DPI-aware** (Per-Monitor V2) para telas de alta resolução.
- **Suíte de testes automatizados** (MSTest) cobrindo as funções de cálculo puro (`CurveFunctions`, `PrepareArrays`, `WriteRawDataToFile`, `DataInputFileReader`).

## Compilando

Requer Visual Studio 2022 (ou Build Tools) com suporte a VB.NET / .NET Framework 4.8.

```
msbuild "SimpleDyno 6.5.vbproj" /t:Build /p:Configuration=Debug
```

O executável final fica em `bin\Debug\SimpleDyno.exe`.

## Testando sem hardware físico

Veja [`tools/arduino-simulator/`](tools/arduino-simulator/README.md) — simula o Arduino via par de portas seriais virtuais (com0com), permitindo validar parsing/calibração dos campos de dados e troca de baud rate sem precisar reconectar o hardware real a cada teste. Não substitui o teste físico completo (não emula corrupção elétrica de baud rate incompatível).

## Documentação original

- [Instruções de uso (PDF)](Instructions%20and%20Docs%20and%20Release%20Zip%20File/Instructions%20for%20using%20SimpleDyno%206.5.pdf)
- [Sketch do Arduino](Instructions%20and%20Docs%20and%20Release%20Zip%20File/SimpleDyno%20Arduino%20Sketch/SimpleDyno_Sketch.ino)
