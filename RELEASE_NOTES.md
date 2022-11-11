v0.11.0.0 - Nov 11 2022
- Add `Output` as CustomOperation with different types (s. `Fli.Outputs`). (https://github.com/CaptnCodr/Fli/pull/37 https://github.com/CaptnCodr/Fli/pull/39 https://github.com/CaptnCodr/Fli/pull/40 https://github.com/CaptnCodr/Fli/pull/41)
- Add .NET7 support and drop .MET5. (https://github.com/CaptnCodr/Fli/pull/42)
- Update dependencies.

v0.9.0.0 - Oct 18 2022
- Add `Output.Id` from `Process.Id`. (https://github.com/CaptnCodr/Fli/pull/27)
- Add `WSL` to provided `Shells`. (https://github.com/CaptnCodr/Fli/pull/31)
- Enhencement: Trim output texts at the end. (https://github.com/CaptnCodr/Fli/pull/32)
- Add logo for Nuget package. (https://github.com/CaptnCodr/Fli/pull/34)
- Update dependencies.

v0.8.0.0 - Oct 12 2022
- Add `Command.executeAsync` for `NET5` and up! (https://github.com/CaptnCodr/Fli/pull/19)
- Add `Error` from `StandardError`. (https://github.com/CaptnCodr/Fli/pull/20)
- Add `Input` for `StandardInput`. (https://github.com/CaptnCodr/Fli/pull/21)
- Fix some build warnings. (https://github.com/CaptnCodr/Fli/pull/22)

v0.7.0.0 - Oct 07 2022
- Add `Encoding` to contexts. (https://github.com/CaptnCodr/Fli/pull/11)
- Renaming of contexts.
- Update docs with more snippets etc.
- **BREAKING** Add output type with exit code: `type Output = { Text: string; ExitCode: int }` (https://github.com/CaptnCodr/Fli/pull/14)

v0.6.1.0 - Oct 04 2022
- Fix: Wrong (old) content in Release 0.6.0 Nuget package.

v0.6.0.0 - Oct 04 2022
- Add `WorkingDirectory` to both ShellContext &amp; ProgramContext. (https://github.com/CaptnCodr/Fli/pull/4)
- Add `Verb` to ProgramContext. (https://github.com/CaptnCodr/Fli/pull/6)
- Add `Username` to ProgramContext. (https://github.com/CaptnCodr/Fli/pull/5)
  + `Credentials` added later (for windows only)
- Add `EnvironmentVariables` to contexts. (https://github.com/CaptnCodr/Fli/pull/8)
- Add internal method `configureProcess` for better testing. (https://github.com/CaptnCodr/Fli/pull/9)

v0.0.2.0 - "Hello World!"-Release Sep 29 2022
- Initial release