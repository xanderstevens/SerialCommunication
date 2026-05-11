Repository overview

This repository contains two related projects in the same tree:
- A Windows Forms C# application (SerialCommunication) that provides a UI for configuring and interacting with a microcontroller over a serial port.
- An Arduino sketch (SerialCommunication.ino) and supporting C code (SerialCommand.* and analog.c) that implements a simple text-based serial command protocol to exercise digital I/O, PWM, and analog reads.

Build / test / lint commands

Windows (C#):
- Build: msbuild SerialCommunication\SerialCommunication.csproj
- Alternative (if .NET SDK supports it): dotnet build SerialCommunication\SerialCommunication.csproj
- Run in Visual Studio by opening the .csproj.
- There are no automated tests or lint rules configured in this repo.

Arduino (sketch):
- Compile with Arduino CLI (recommended):
  - arduino-cli compile --fqbn arduino:avr:uno SerialCommunication/SerialCommunication.ino
  - arduino-cli upload -p COM3 --fqbn arduino:avr:uno SerialCommunication/SerialCommunication.ino

Notes on running a single test: there is currently no test framework in this repo. If unit tests are later added, run them using the project-specific test runner (e.g., dotnet test) — add instructions here as they are added.

High-level architecture

- UI (C# WinForms): Form1.cs (and Form1.Designer.cs) builds a tabbed interface exposing: serial port selection and settings, several exercise tabs for digital outputs, PWM control (trackbars), analog readouts, and a simple thermostat example. Program.cs simply starts the WinForms app.
- Serial protocol (Arduino): SerialCommunication.ino uses the included SerialCommand library to parse text commands received over Serial. The Arduino exposes commands like:
  - set dX <on|off|high|low|1|0>
  - set pwmY <0..255>
  - toggle dX
  - get dX
  - get aX
  - ping / pong
  - debug / help
- Utility library: SerialCommand.h/cpp implements a small tokenizing command parser (buffer size and command count are configured via #defines). analog.c contains analogReadDelay used by the sketch for delayed analog sampling.

Key conventions and repository-specific patterns

- Mixed-target layout: C# WinForms and Arduino sketch live in the same repo and folder. Treat them as separate build targets — desktop app is built with MSBuild/Visual Studio; firmware is compiled/uploaded with Arduino tooling.
- Serial protocol details (important for UI <> firmware integration):
  - Default baudrate used in the sketch: 115200. The UI sets its baudrate combobox default to 115200.
  - Digital pins: commands use prefix "d" (digital). Valid d pin ranges in the sketch: 2..7 for reads, 2..4 for set/toggle.
  - PWM pins: commands use prefix "pwm" (e.g., pwm9). PWM range: 9..11 with values 0..255.
  - Analog pins: prefix "a" (a0..a5). The sketch returns "aN: <value>" and "dN: <value>" lines for reads.
  - Responses: the firmware returns short human-readable messages (e.g., "set done", "toggle done", "pong", or error lines); the UI expects these concise tokens.
  - SerialCommand specifics: buffer size (SERIALCOMMANDBUFFER), max commands (MAXSERIALCOMMANDS), and default terminator are defined in SerialCommand.* — changes to those affect protocol parsing and must be kept in sync with any UI timeouts/behavior.
- Terminator vs echo: SerialCommand.cpp sets term='\n' and echoes input when SERIALCOMMANDDEBUG is enabled; note mismatch with some header comments that mention '\r'. Use the implementation as source-of-truth.

Files to consult for quick context

- SerialCommunication/SerialCommunication.ino — firmware entry point and command handlers
- SerialCommunication/SerialCommand.h/.cpp — firmware command parser
- SerialCommunication/Form1.cs and Form1.Designer.cs — UI behavior and control wiring

AI / assistant considerations

- When editing protocol handlers in the Arduino sketch, update this file and mention the corresponding UI changes (pin ranges, expected responses) so future Copilot sessions can modify both sides consistently.
- There are no existing CI checks, tests, or linter configs to copy. If adding CI or linters, add examples here (build commands, test commands, and where to run them).

Other AI assistant configs

No CLAUDE.md, .cursorrules, AGENTS.md, .windsurfrules, CONVENTIONS.md, or similar assistant config files were found in the repository root. If such files are added, include any actionable rules here (e.g., rules that affect commit messages, branch naming, or automated fixes).

MCP servers

Would you like to configure any MCP servers (e.g., an Arduino/embedded test runner or a serial device emulator) for this project? Reply and the assistant can add a basic MCP configuration example.

--
Generated copilot-instructions.md: covers build commands present today, architecture summary, and repository-specific conventions. Update this file when adding tests, CI, or changing the serial protocol.
