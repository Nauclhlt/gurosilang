namespace Gurosi;

public sealed class StatementBlock : Statement
{
    private List<Statement> _statements;
    private Token _startToken;

    public List<Statement> Statements => _statements;
    public Token StartToken => _startToken;

    public StatementBlock(List<Statement> statements, Token startToken)
    {
        _startToken = startToken;
        _statements = statements;
    }

    internal override string _PrintDebug()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('{');
        sb.Append(Environment.NewLine);
        if (_statements.Count > 0)
            sb.Append(_statements[0]._PrintDebug());
        for (int i = 1; i < _statements.Count; i++)
        {
            sb.Append(Environment.NewLine);
            sb.Append(_statements[i]._PrintDebug());
        }
        sb.Append(Environment.NewLine);
        sb.Append('}');

        return sb.ToString();
    }
}