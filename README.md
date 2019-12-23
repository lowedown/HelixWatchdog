# HelixWatchdog
Command line application that validates Helix references inside the 'src' folder. This tool checks for any kind of reference by checking .cs, .csproj, .config files for strings.

## Usage

    helixWatchdog.exe --source "c:\dev\myproject\src\" --pattern "*.cs|*.csproj|*.config" --namespace "My.Name.Space"

## Examples

    MyNamespace.Feature.ThisFeature.SomeClass ==> valid         // Valid in Feature.ThisFeature
    MyNamespace.Foundation.SomeModule.SomeClass ==> valid       // Valid in Feature.ThisFeature
    MyNamespace.Feature.OtherFeature    ==> invalid             // No feature can reference other features
    
    
Best Practice: To make the most of this validation tool, ensure that settings, pipelines and other configs are prefixed with the Helix scheme. Example:

    <setting name="MyNamespace.Feature.SomeModule.SomeSetting" value="some value" />
    
This way, no other feature can use this setting ensuring maximum separation of concerns.

