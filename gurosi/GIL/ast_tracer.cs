namespace Gurosi;

public static class AstTracer
{
    public static bool CheckIfAllPathsReturns(StatementBlock block)
    {
        for (int i = 0; i < block.Statements.Count; i++)
        {
            Statement s = block.Statements[i];

            if (s is ReturnStatement || s is ErrorStatement)
                return true;
            
            if (s is IfStatement ifs)
            {
                if (ifs.ElseBody is not null)
                {
                    if (CheckIfAllPathsReturns(ifs.Body) &&
                        CheckIfAllPathsReturns(ifs.ElseBody))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (s is StatementBlock sb)
            {
                if (CheckIfAllPathsReturns(sb))
                    return true;
            }
        }

        return false;
    }
}