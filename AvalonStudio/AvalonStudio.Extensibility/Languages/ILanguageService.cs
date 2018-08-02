using AvalonStudio.Documents;
using AvalonStudio.Editor;
using AvalonStudio.Extensibility.Languages;
using AvalonStudio.Extensibility.Languages.CompletionAssistance;
using AvalonStudio.Projects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace AvalonStudio.Languages
{
    public enum DiagnosticsUpdatedKind
    {
        DiagnosticsRemoved,
        DiagnosticsCreated
    }

    public class DiagnosticsUpdatedEventArgs : EventArgs
    {
        public DiagnosticsUpdatedEventArgs(object tag, DiagnosticsUpdatedKind kind)
        {
            Tag = tag;
            Kind = kind;
            Source = DiagnosticSourceKind.Misc;
        }

        public DiagnosticsUpdatedEventArgs(object tag, string filePath, DiagnosticsUpdatedKind kind, DiagnosticSourceKind source, ImmutableArray<Diagnostic> diagnostics, SyntaxHighlightDataList diagnosticHighlights = null)
        {
            Tag = tag;
            FilePath = filePath;
            Kind = kind;
            Diagnostics = diagnostics;
            DiagnosticHighlights = diagnosticHighlights;
            Source = source;
        }

        public object Tag { get; }
        public string FilePath { get; }
        public DiagnosticsUpdatedKind Kind { get; }
        public DiagnosticSourceKind Source { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public SyntaxHighlightDataList DiagnosticHighlights { get; }
    }

    public interface ILanguageService
    {
        /// <summary>
        /// A file path compatible name for the language, i.e. cs, cpp, ts, css, go, vb, fsharp
        /// </summary>
        string LanguageId { get; }

        /// <summary>
        /// Dictionary of functions for transforming snippet variables. Key is function name, the arugment is the string to transform.
        /// i.e. (propertyName) => "_" + propertyName
        /// </summary>
        IDictionary<string, Func<string, string>> SnippetCodeGenerators { get; }

        /// <summary>
        /// Dictionary of dynamic varables that can be evaluated by snippets. i.e. ClassName, arguments are CaretIndex, Line, Column.
        /// </summary>
        IDictionary<string, Func<int, int, int, string>> SnippetDynamicVariables { get; }

        bool CanTriggerIntellisense(char currentChar, char previousChar);
        IEnumerable<char> IntellisenseSearchCharacters { get; }
        IEnumerable<char> IntellisenseCompleteCharacters { get; }
        IEnumerable<ITextEditorInputHelper> InputHelpers { get; }

        bool IsValidIdentifierCharacter(char data);

        Task<CodeCompletionResults> CodeCompleteAtAsync(ITextEditor editor, int index, int line, int column, List<UnsavedFile> unsavedFiles, char lastChar, string filter = "");

        Task<CodeAnalysisResults> RunCodeAnalysisAsync(ITextEditor editor, List<UnsavedFile> unsavedFiles, Func<bool> interruptRequested);        

        IEnumerable<IContextActionProvider> GetContextActionProviders(ITextEditor editor);

        Task<SignatureHelp> SignatureHelp(ITextEditor editor, List<UnsavedFile> unsavedFiles, int offset, string methodName);

        Task<QuickInfoResult> QuickInfo(ITextEditor editor, List<UnsavedFile> unsavedFiles, int offset);

        Task<List<Symbol>> GetSymbolsAsync(ITextEditor editor, List<UnsavedFile> unsavedFiles, string name);

        Task<GotoDefinitionInfo> GotoDefinition(ITextEditor editor, int offset);

        Task<IEnumerable<SymbolRenameInfo>> RenameSymbol(ITextEditor editor, string renameTo, bool initalise = false);

        void RegisterSourceFile(ITextEditor editor);

        void UnregisterSourceFile(ITextEditor editor);

        bool CanHandle(ITextEditor editor);

        int Format(ITextEditor editor, uint offset, uint length, int cursor);

        int Comment(ITextEditor editor, int firstLine, int endLine, int caret = -1, bool format = true);

        int UnComment(ITextEditor editor, int firstLine, int endLine, int caret = -1, bool format = true);
    }
}