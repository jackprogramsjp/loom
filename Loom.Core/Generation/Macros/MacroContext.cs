using Loom.SemanticAnalysis;

namespace Loom.Generation.Macros;

internal readonly record struct MacroContext(SemanticModel SemanticModel, LuauState State);