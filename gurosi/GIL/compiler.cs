namespace Gurosi;

public sealed class Compiler
{
    private static Dictionary<int, string> CalcMap = new Dictionary<int, string>()
    {
        { (int)Operator.Add, "add" },
        { (int)Operator.Sub, "sub" },
        { (int)Operator.Mult, "mul" },
        { (int)Operator.Div, "div" },
        { (int)Operator.Mod, "mod" },
        { (int)Operator.LogAnd, "and" },
        { (int)Operator.LogOr, "or" },
        { (int)Operator.Lt, "lt" },
        { (int)Operator.Lte, "lte" },
        { (int)Operator.Gt, "gt" },
        { (int)Operator.Gte, "gte" },
        { (int)Operator.Equal, "eq" },
        { (int)Operator.NotEqual, "neq" },
    };

    public Compiler()
    {
    }

    public CompileResult CompileRunBlock(StatementBlock block, RuntimeEnv runtime)
    {
        CompileResult m = CompileRunBlockCode(block, runtime);

        return new CompileResult(m.Code, m.Errors);
    }

    public CompileResult CompileFunction(Function model, RuntimeEnv runtime)
    {
        List<Error> errors = new List<Error>();

        FunctionBinary fb = new FunctionBinary();
        fb.Module = model.Module;
        fb.Name = model.Name;
        fb.ReturnType = runtime.Interpolate(TypePath.FromModel(model.ReturnType));

        if (fb.ReturnsValueR && !runtime.TypeExists(fb.ReturnType))
        {
            errors.Add(new Error(ErrorProvider.InvalidType(fb.ReturnType), model.Token.Point));
        }
        

        fb.Identifiers = new List<AccessIdentifier>();
        fb.Arguments = model.Parameters.Select(x =>
        {
            TypePath type = runtime.Interpolate(TypePath.FromModel(x.Type));

            if (!runtime.TypeExists(type))
            {
                errors.Add(new Error(ErrorProvider.InvalidType(type), x.Token.Point));
            }

            return ArgumentBinary.FromModel(x, runtime);
        }).ToList();


        if (model.ExtendType is null)
        {
            fb.ExtendType = null;
            fb.ExtendName = null;
        }
        else
        {
            TypePath extType = TypePath.FromModel(model.ExtendType);
            if (!runtime.TypeExists(extType))
            {
                errors.Add(new Error(ErrorProvider.InvalidType(extType), model.Token.Point));
            }
            fb.ExtendType = extType;
            fb.ExtendName = model.ExtendName;
        }



        CompileResult m = CompileFuncCode(model.Body, runtime, fb);
        fb.Body = new CodeBinary(m.Code);
        errors.AddRange(m.Errors);

        return new CompileResult(fb, errors);
    }

    public CompileResult CompileClass(ClassModel model, RuntimeEnv runtime)
    {
        List<Error> errors = new List<Error>();
        FunctionIndexer findexer = new FunctionIndexer();

        ClassBinary cb = new ClassBinary();

        if (model.BaseType is not null)
            cb.BaseType = runtime.Interpolate(TypePath.FromModel(model.BaseType));

        

        cb.Path = new TypePath(model.Module, model.Name);
        cb.Identifiers = model.Identifiers.ToList();
        cb.Fields = model.Fields.Select(x =>
        {
            TypePath type = runtime.Interpolate(TypePath.FromModel(x.Type));
            
            if (!runtime.TypeExists(type))
            {
                errors.Add(new Error(ErrorProvider.InvalidType(type), x.Token.Point));
            }

            return FieldBinary.FromModel(x, runtime);
        }).ToList();

        if (model.BaseType is not null)
        {
            // add base class fields
            if (!runtime.IsClass(cb.BaseType))
            {
                errors.Add(new Error(ErrorProvider.InvalidInheritance(cb.BaseType), default));
            }
            else
            {
                ClassBinary sourceClass = runtime.GetClass(cb.BaseType);

                cb.Fields.AddRange(sourceClass.Fields);
            }
        }

        cb.StcFields = model.StcFields.Select(x =>
        {
            TypePath type = runtime.Interpolate(TypePath.FromModel(x.Type));
            
            if (!runtime.TypeExists(type))
            {
                errors.Add(new Error(ErrorProvider.InvalidType(type), x.Token.Point));
            }

            return FieldBinary.FromModel(x, runtime);
        }).ToList();

        cb.Functions = model.Methods.Select(x =>
        {
            FunctionBinary fb = new FunctionBinary();
            fb.Module = model.Module;
            fb.Name = x.Name;
            fb.ReturnType = runtime.Interpolate(TypePath.FromModel(x.ReturnType));

            if (fb.ReturnsValueR && !runtime.TypeExists(fb.ReturnType))
            {
                errors.Add(new Error(ErrorProvider.InvalidType(fb.ReturnType), x.Token.Point));
            }

            fb.Identifiers = x.AccessIdentifiers.ToList();
            fb.Arguments = x.Parameters.Select(x =>
            {
                TypePath type = runtime.Interpolate(TypePath.FromModel(x.Type));

                if (!runtime.TypeExists(type))
                {
                    errors.Add(new Error(ErrorProvider.InvalidType(type), x.Token.Point));
                }

                return ArgumentBinary.FromModel(x, runtime);
            }).ToList();

            CompileResult m = CompileMethodCode(x.Body, runtime, fb, ClassBinary.CreatePrototype(model, runtime));
            errors.AddRange(m.Errors);
            fb.Body = new CodeBinary(m.Code);
            
            return fb;
        }).ToList();

        // extend type
        if (cb.BaseType is not null)
        {
            ClassBinary ext = runtime.GetClass(cb.BaseType);
            ClassModel extModel = ext.PrototypeSource;

            cb.Functions.AddRange(extModel.Methods.Where(a => !a.Name.StartsWith("ctor~")).Select(x =>
            {
                FunctionBinary fb = new FunctionBinary();
                fb.Module = model.Module;
                fb.Name = x.Name;
                fb.ReturnType = runtime.Interpolate(TypePath.FromModel(x.ReturnType));

                if (fb.ReturnsValueR && !runtime.TypeExists(fb.ReturnType))
                {
                    errors.Add(new Error(ErrorProvider.InvalidType(fb.ReturnType), x.Token.Point));
                }

                fb.Identifiers = x.AccessIdentifiers.ToList();
                fb.Arguments = x.Parameters.Select(x =>
                {
                    TypePath type = runtime.Interpolate(TypePath.FromModel(x.Type));

                    if (!runtime.TypeExists(type))
                    {
                        errors.Add(new Error(ErrorProvider.InvalidType(type), x.Token.Point));
                    }

                    return ArgumentBinary.FromModel(x, runtime);
                }).ToList();

                CompileResult m = CompileMethodCode(x.Body, runtime, fb, ClassBinary.CreatePrototype(model, runtime));
                errors.AddRange(m.Errors);
                fb.Body = new CodeBinary(m.Code);
                
                return fb;
            }));

            cb.Fields.AddRange(cb.Fields);
        }



        if (model.BaseType is not null)
        {
            // add base class fields
            if (!runtime.IsClass(cb.BaseType))
            {
                errors.Add(new Error(ErrorProvider.InvalidInheritance(cb.BaseType), default));
            }
            else
            {
                ClassBinary sourceClass = runtime.GetClass(cb.BaseType);

                // add functions;
                for (int i = 0; i < sourceClass.Functions.Count; i++)
                {
                    cb.Functions.Add(sourceClass.Functions[i]);
                }
            }
        }

        cb.StcFunctions = model.StcMethods.Select(x =>
        {
            FunctionBinary fb = new FunctionBinary();
            fb.Name = x.Name;
            fb.Module = model.Module;
            
            fb.ReturnType = runtime.Interpolate(TypePath.FromModel(x.ReturnType));

            if (fb.ReturnsValueR && !runtime.TypeExists(fb.ReturnType))
            {
                errors.Add(new Error(ErrorProvider.InvalidType(fb.ReturnType), x.Token.Point));
            }

            fb.Identifiers = x.AccessIdentifiers.ToList();
            fb.Arguments = x.Parameters.Select(x =>
            {
                TypePath type = runtime.Interpolate(TypePath.FromModel(x.Type));

                if (!runtime.TypeExists(type))
                {
                    errors.Add(new Error(ErrorProvider.InvalidType(type), x.Token.Point));
                }

                return ArgumentBinary.FromModel(x, runtime);
            }).ToList();

            CompileResult m = CompileMethodCode(x.Body, runtime, fb, cb, isStatic: true);
            errors.AddRange(m.Errors);
            fb.Body = new CodeBinary(m.Code);
            
            return fb;
        }).ToList();

        cb.GenericCount = model.GenericCount;

        

        return new CompileResult(cb, errors);
    }

    public CompileResult CompileMethodCode(StatementBlock block, RuntimeEnv runtime, FunctionBinary func, ClassBinary root,
                                            bool isStatic = false)
    {
        CompileEnvironment env = new CompileEnvironment();
        CompileContext ctx = isStatic ? CompileContext.CreateClassContextStatic(runtime, root)
                                       : CompileContext.CreateClassContext(runtime, root);
        ctx.MakeFunctionContext(func);

        if (func.ReturnsValueR && !AstTracer.CheckIfAllPathsReturns(block))
        {
            env.Errors.Add(new Error(ErrorProvider.NotAllPathReturns(), block.StartToken.Point));
        }

        CompileStatement(env, ctx, block);

        return new CompileResult(env);
    }

    public CompileResult CompileFuncCode(StatementBlock block, RuntimeEnv runtime, FunctionBinary root)
    {
        CompileEnvironment env = new CompileEnvironment();
        CompileContext ctx = CompileContext.CreateRootEmpty(runtime);
        ctx.MakeFunctionContext(root);

        if (root.ReturnsValueR && !AstTracer.CheckIfAllPathsReturns(block))
        {
            env.Errors.Add(new Error(ErrorProvider.NotAllPathReturns(), block.StartToken.Point));
        }

        CompileStatement(env, ctx, block);

        return new CompileResult(env);
    }

    private CompileResult CompileRunBlockCode(StatementBlock block, RuntimeEnv runtime)
    {
        CompileEnvironment env = new CompileEnvironment();
        CompileContext ctx = CompileContext.CreateRootEmpty(runtime);

        CompileStatement(env, ctx, block);

        return new CompileResult(env);
    }

    private void CompileStatement(CompileEnvironment env, CompileContext c, Statement statement)
    {
        if (statement is StatementBlock sb)
        {
            c.CreateNewScope();
            for (int i = 0; i < sb.Statements.Count; i++)
            {
                CompileStatement(env, c, sb.Statements[i]);
            }
            c.BreakLastScope();
        }
        else if (statement is LetStatement ls)
        {
            env.Code.Add("alloc");
            int addr = c.AllocAddr();
            env.Code.Add(addr.ToString());
            TypePath type = c.Runtime.Interpolate(TypePath.FromModel(ls.Type));
            env.Code.Add(type.ToString());

            // type validation:
            if ( !c.Runtime.TypeExists(type) )
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidType(type), ls.Token.Point));
                return;
            }

            if (ls.Value is not null)
            {
                // value type check:
                TypePath valueType = TypeEvaluator.Evaluate(ls.Value, c.Runtime, c);
                bool validType = valueType.IsCompatibleWith(type, c.Runtime, false);
                if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, type)))
                {
                    //Console.WriteLine(valueType);
                    
                    env.Errors.Add(new Error(ErrorProvider.LetTypeMismatch(ls.VarName, type), ls.Token.Point));
                    return;
                }

                CompileExpression(env, c, ls.Value);

                // cast
                if (!validType)
                {
                    env.Code.Add(TypeEvaluator.CastMap[(valueType, type)]);
                }

                env.Code.Add("pop");
                env.Code.Add(addr.ToString());
            }
            

            if (c.IsLocalDeclared(ls.VarName))
            {
                env.Errors.Add(new Error(ErrorProvider.LocalNameDuplicate(ls.VarName), ls.Token.Point));
            }

            if (c.HasClass && ((!c.Static && c.ScopeClass.HasField(ls.VarName)) ||
                                c.ScopeClass.HasStcField(ls.VarName)))
            {
                env.Errors.Add(new Error(ErrorProvider.LocalNameFieldConflict(ls.VarName), ls.Token.Point));
            }

            c.DeclareLocal(ls.VarName, type, addr);
        }
        else if (statement is ConstStatement cs)
        {
            env.Code.Add("alloc");
            int addr = c.AllocAddr();
            env.Code.Add(addr.ToString());
            TypePath type = c.Runtime.Interpolate(TypePath.FromModel(cs.Type));
            env.Code.Add(type.ToString());

            // type validation:
            if ( !c.Runtime.TypeExists(type) )
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidType(type), cs.Token.Point));
                return;
            }

            if (cs.Value is not null)
            {
                // value type check:
                TypePath valueType = TypeEvaluator.Evaluate(cs.Value, c.Runtime, c);
                bool validType = valueType.IsCompatibleWith(type, c.Runtime, false); ;
                if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, type)))
                {
                    //Console.WriteLine(valueType);
                    
                    env.Errors.Add(new Error(ErrorProvider.LetTypeMismatch(cs.VarName, type), cs.Token.Point));
                    return;
                }

                CompileExpression(env, c, cs.Value);

                // cast
                if (!validType)
                {
                    env.Code.Add(TypeEvaluator.CastMap[(valueType, type)]);
                }

                env.Code.Add("pop");
                env.Code.Add(addr.ToString());
            }
            

            if (c.IsLocalDeclared(cs.VarName))
            {
                env.Errors.Add(new Error(ErrorProvider.LocalNameDuplicate(cs.VarName), cs.Token.Point));
            }

            if (c.HasClass && ((!c.Static && c.ScopeClass.HasField(cs.VarName)) ||
                                c.ScopeClass.HasStcField(cs.VarName)))
            {
                env.Errors.Add(new Error(ErrorProvider.LocalNameFieldConflict(cs.VarName), cs.Token.Point));
            }

            c.DeclareLocalConst(cs.VarName, type, addr);
        }
        else if (statement is IfStatement ifs)
        {
            TypePath conditionType = TypeEvaluator.Evaluate(ifs.Condition, c.Runtime, c);
            if (!conditionType.IsCompatibleWith(TypePath.BOOLEAN, c.Runtime))
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidConditionType(), ifs.Condition.Token.Point));
            }

            CompileExpression(env, c, ifs.Condition);
            env.Code.Add("njmp");
            int offsetIndex = env.Code.Count;
            env.Code.Add(string.Empty);

            if (ifs.ElseBody is null)
            {
                int bodylen = env.Code.Count;
                c.CreateNewScope();
                CompileStatement(env, c, ifs.Body);
                c.BreakLastScope();
                bodylen = GIL.CountInstructions(env.Code, bodylen, env.Code.Count - bodylen);

                env.Code[offsetIndex] = bodylen.ToString();
            }
            else
            {
                int bodylen = env.Code.Count;
                c.CreateNewScope();
                CompileStatement(env, c, ifs.Body);
                env.Code.Add("jmp");
                int eloffsetIndex = env.Code.Count;
                env.Code.Add(string.Empty);
                c.BreakLastScope();
                bodylen = GIL.CountInstructions(env.Code, bodylen, env.Code.Count - bodylen);

                env.Code[offsetIndex] = bodylen.ToString();

                bodylen = env.Code.Count;
                c.CreateNewScope();
                CompileStatement(env, c, ifs.ElseBody);
                c.BreakLastScope();
                bodylen = GIL.CountInstructions(env.Code, bodylen, env.Code.Count - bodylen);
                env.Code[eloffsetIndex] = bodylen.ToString();
            }
        }
        else if (statement is AssignStatement assign)
        {
            TypePath valueType = TypeEvaluator.Evaluate(assign.Value, c.Runtime, c);

            CompileValueSetting(env, c, assign.Target, valueType, assign.Target.Token, () =>
            {
                CompileExpression(env, c, assign.Value);
            });
        }
        else if (statement is WhileStatement ws)
        {
            int bodylen = env.Code.Count;
            c.CreateNewLoopScope();
            
            CompileStatement(env, c, ws.Body);

            env.Code.Add("cpl");
            CompileExpression(env, c, ws.Condition);
            env.Code.Add("cjmp");
            int offsetIndex = env.Code.Count;
            env.Code.Add(string.Empty);
            c.BreakLastScope();

            bodylen = GIL.CountInstructions(env.Code, bodylen, env.Code.Count - bodylen);
            env.Code[offsetIndex] = (-bodylen + 1).ToString();

            env.Code.Add("bpl");
        }
        else if (statement is ForStatement fs)
        {
            c.CreateNewLoopScope();
            {
                int addr = c.AllocAddr();
                env.Code.Add("alloc");
                env.Code.Add(addr.ToString());
                env.Code.Add(TypePath.INT.ToString());
                CompileExpression(env, c, fs.From);
                env.Code.Add("pop");
                env.Code.Add(addr.ToString());
                c.DeclareLocal(fs.VarName, TypePath.INT, addr);

                int bodylen = env.Code.Count;
                
                CompileStatement(env, c, fs.Body);

                env.Code.Add("cpl");
                // i++
                env.Code.Add("push");
                env.Code.Add(addr.ToString());
                env.Code.Add("pushint");
                env.Code.Add("1");
                env.Code.Add("add");
                env.Code.Add("pop");
                env.Code.Add(addr.ToString());
                // end i++

                env.Code.Add("push");
                env.Code.Add(addr.ToString());
                CompileExpression(env, c, fs.To);
                env.Code.Add("lt");
                bodylen = GIL.CountInstructions(env.Code, bodylen, env.Code.Count - bodylen);

                env.Code.Add("cjmp");
                env.Code.Add((-bodylen).ToString());

                env.Code.Add("bpl");
            }
            c.BreakLastScope();
        }
        else if (statement is FuncStatement func)
        {
            CompileExpression(env, c, func.Inner);
            FunctionBinary function = TypeEvaluator.FindFunction(func.Inner, c.Runtime, c);

            if (function is not null && function.ReturnsValueR)
                env.Code.Add("disc");
        }
        else if (statement is ReturnStatement ret)
        {
            if (ret.Value is not null && !c.ScopeFunction.ReturnsValueR)
            {
                // return value not required, but passed.
                env.Errors.Add(new Error(ErrorProvider.ReturnValueNotRequired(), ret.ValueToken.Point));
            }
            if (ret.Value is not null && c.ScopeFunction.ReturnsValueR && !TypeEvaluator.Evaluate(ret.Value, c.Runtime, c).IsCompatibleWith(c.ScopeFunction.ReturnType, c.Runtime))
            {
                env.Errors.Add(new Error(ErrorProvider.ReturnTypeMismatch(c.ScopeFunction.ReturnType), ret.ValueToken.Point));
            }
            if (c.HasFunction && c.ScopeFunction.ReturnsValueR &&
                ret.Value is null)
            {
                // return value required, but not passed.
                env.Errors.Add(new Error(ErrorProvider.ReturnValueMissing(), ret.ValueToken.Point));
            }

            CompileExpression(env, c, ret.Value);
            env.Code.Add("ret");
        }
        else if (statement is BreakStatement brk)
        {
            if (!c.CurrentlyInLoop)
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidBreak(), brk.Token.Point));
            }
            env.Code.Add("brk");
        }
        else if (statement is ContinueStatement cont)
        {
            if (!c.CurrentlyInLoop)
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidContinue(), cont.Token.Point));
            }
            env.Code.Add("cont");
        }
        else if (statement is ScopebreakStatement scbrk)
        {
            if (c.IsRoot)
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidScopebreak(), scbrk.Token.Point));
            }

            // TODO: implement.
        }
        else if (statement is NvCallStatement nvc)
        {
            for (int i = 0; i < nvc.Arguments.Count; i++)
            {
                CompileExpression(env, c, nvc.Arguments[i]);
            }

            env.Code.Add("nvc");
            env.Code.Add(nvc.Asm);
            env.Code.Add(nvc.Cls);
            env.Code.Add(nvc.Symbol);
        }
        else if (statement is ErrorStatement err)
        {
            TypePath codeType = TypeEvaluator.Evaluate(err.ErrorCode, c.Runtime, c);
            TypePath msgType = TypeEvaluator.Evaluate(err.ErrorMessage, c.Runtime, c);

            if (!codeType.CompareEquality(TypePath.STRING) ||
                !msgType.CompareEquality(TypePath.STRING))
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidErrorType(), err.Token.Point));
            }

            CompileExpression(env, c, err.ErrorCode);
            CompileExpression(env, c, err.ErrorMessage);
            env.Code.Add("err");
        }
        else if (statement is IncrStatement incr)
        {
            TypePath valueType = TypeEvaluator.Evaluate(incr.Inner.Target, c.Runtime, c);
            if (!TypeEvaluator.Incrementable.Contains(valueType))
            {
                env.Errors.Add(new Error(ErrorProvider.SingleOperatorTypeMismatch(valueType, "++"), incr.Inner.Token.Point));
            }

            CompileValueSetting(env, c, incr.Inner.Target, valueType, incr.Inner.Token, () =>
            {
                CompileExpression(env, c, incr.Inner.Target);
                env.Code.Add("inc");
            });
        }
        else if (statement is DecrStatement decr)
        {
            TypePath valueType = TypeEvaluator.Evaluate(decr.Inner.Target, c.Runtime, c);
            if (!TypeEvaluator.Incrementable.Contains(valueType))
            {
                env.Errors.Add(new Error(ErrorProvider.SingleOperatorTypeMismatch(valueType, "--"), decr.Inner.Token.Point));
            }

            CompileValueSetting(env, c, decr.Inner.Target, valueType, decr.Inner.Token, () =>
            {
                CompileExpression(env, c, decr.Inner.Target);
                env.Code.Add("dec");
            });
        }
    }

    private void CompileValueSetting(CompileEnvironment env, CompileContext c, Expression target, TypePath valueType, Token errorToken,
                                    Action valueCompiler)
    {
        if (target is IdentExpression ident)
        {
            if (c.TryGetLocal(ident.Identifier, out Local lcl))
            {
                // type check

                bool validType = valueType.IsCompatibleWith(lcl.Type, c.Runtime, false);
                if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, lcl.Type)))
                {
                    env.Errors.Add(new Error(ErrorProvider.LocalAssignTypeMismatch(ident.Identifier, lcl.Type), errorToken.Point));
                    return;
                }

                if (lcl.Constant)
                {
                    env.Errors.Add(new Error(ErrorProvider.InvalidConstAssignment(), target.Token.Point));
                }

                // set local variable
                //CompileExpression(env, c, assign.Value);
                valueCompiler();

                if (!validType)
                {
                    env.Code.Add(TypeEvaluator.CastMap[(valueType, lcl.Type)]);
                }

                env.Code.Add("pop");
                env.Code.Add(lcl.Address.ToString());
            }
            else
            {
                // class field ?
                if (c.HasClass && !c.Static && c.ScopeClass.HasField(ident.Identifier))
                {
                    FieldBinary field = c.ScopeClass.GetField(ident.Identifier);

                    if (!c.IsFieldAccessible(c.ScopeClass, field))
                    {
                        env.Errors.Add(new Error(ErrorProvider.NotAccessible(field.Name), ident.Token.Point));
                    }

                    bool validType = valueType.IsCompatibleWith(field.Type, c.Runtime, false);

                    if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, field.Type)))
                    {
                        env.Errors.Add(new Error(ErrorProvider.FieldAssignTypeMismatch(ident.Identifier, field.Type), errorToken.Point));
                        return;
                    }
                    
                    env.Code.Add("self");
                    
                    valueCompiler();

                    if (!validType)
                    {
                        env.Code.Add(TypeEvaluator.CastMap[(valueType, field.Type)]);
                    }

                    env.Code.Add("popf");
                    env.Code.Add(c.ScopeClass.GetFieldIndex(ident.Identifier).ToString());
                }
                else if (c.HasClass && c.ScopeClass.HasStcField(ident.Identifier))
                {
                    FieldBinary stc = c.ScopeClass.GetStcField(ident.Identifier);

                    if (!c.IsFieldAccessible(c.ScopeClass, stc))
                    {
                        env.Errors.Add(new Error(ErrorProvider.NotAccessible(stc.Name), ident.Token.Point));
                    }

                    bool validType = valueType.IsCompatibleWith(stc.Type, c.Runtime, false);

                    if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, stc.Type)))
                    {
                        env.Errors.Add(new Error(ErrorProvider.FieldAssignTypeMismatch(ident.Identifier, stc.Type), errorToken.Point));
                        return;
                    }

                    CompilePath(env, c, c.ScopeClass.Path);
                    //CompileExpression(env, c, assign.Value);
                    valueCompiler();
                    
                    if (!validType)
                    {
                        env.Code.Add(TypeEvaluator.CastMap[(valueType, stc.Type)]);
                    }

                    env.Code.Add("popstc");
                    env.Code.Add(c.ScopeClass.GetStcFieldIndex(ident.Identifier).ToString());
                }
                else if (c.Runtime.ModuleExists(ident.Identifier) ||
                        c.Runtime.Interpolate(new TypePath(Array.Empty<string>(), ident.Identifier)) is not null)
                {
                }
                else
                {
                    env.Errors.Add(new Error(ErrorProvider.SymbolNotFoundInScope(ident.Identifier), ident.Token.Point));
                }
            }
        }
        else if (target is DotExpression dot)
        {
            TypePath sourceType = TypeEvaluator.Evaluate(dot.Source, c.Runtime, c);
            TypePath pureType = c.Runtime.Interpolate(TypePath.FromModel(new TypeData(dot.Source)));
            if (sourceType.NotNull() && c.Runtime.TypeExists(sourceType))
            {
                CompileExpression(env, c, dot.Source);
                ClassBinary cls = c.Runtime.GetClass(sourceType);
                if (cls.HasField(dot.Right))
                {
                    FieldBinary field = cls.GetField(dot.Right);
                    if (!c.IsFieldAccessible(cls, field))
                    {
                        env.Errors.Add(new Error(ErrorProvider.NotAccessible(field.Name), dot.Token.Point));
                    }

                    bool validType = valueType.IsCompatibleWith(field.Type, c.Runtime, false);

                    if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, field.Type)))
                    {
                        env.Errors.Add(new Error(ErrorProvider.FieldAssignTypeMismatch(field.Name, field.Type), errorToken.Point));
                        return;
                    }

                    //CompileExpression(env, c, assign.Value);
                    valueCompiler();

                    if (!validType)
                    {
                        env.Code.Add(TypeEvaluator.CastMap[(valueType, field.Type)]);
                    }

                    env.Code.Add("popf");
                    env.Code.Add(cls.GetFieldIndex(dot.Right).ToString());
                }
                else
                {
                    env.Errors.Add(new Error(ErrorProvider.ClassSymbolNotFound(dot.Right, sourceType), dot.Source.Token.Point));
                }
            }
            else if (pureType is not null && c.Runtime.TypeExists(pureType))
            {
                ClassBinary cb = c.Runtime.GetClass(pureType);
                if (cb.HasStcField(dot.Right))
                {
                    FieldBinary stc = cb.GetStcField(dot.Right);

                    if (!c.IsFieldAccessible(cb, stc))
                    {
                        env.Errors.Add(new Error(ErrorProvider.NotAccessible(stc.Name), dot.Token.Point));
                    }

                    bool validType = valueType.IsCompatibleWith(stc.Type, c.Runtime, false);

                    if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, stc.Type)))
                    {
                        env.Errors.Add(new Error(ErrorProvider.FieldAssignTypeMismatch(stc.Name, stc.Type), errorToken.Point));
                        return;
                    }

                    CompilePath(env, c, pureType);
                    //CompileExpression(env, c, assign.Value);
                    valueCompiler();

                    if (!validType)
                    {
                        env.Code.Add(TypeEvaluator.CastMap[(valueType, stc.Type)]);
                    }

                    env.Code.Add("popstc");
                    
                    env.Code.Add(cb.GetStcFieldIndex(dot.Right).ToString());
                }
                else
                {
                    env.Errors.Add(new Error(ErrorProvider.ClassSymbolNotFound(dot.Right, cb.Path), dot.Token.Point));
                }
            }
            else
            {
                env.Errors.Add(new Error("TEMP ERROR", default));
            }
        }
        else if (target is IndexExpression idx)
        {
            TypePath arrayType = TypeEvaluator.Evaluate(idx.Target, c.Runtime, c);

            if (!arrayType.IsArray)
            {
                env.Errors.Add(new Error(ErrorProvider.NotArrayIndexed(), errorToken.Point));
            }
            else
            {
                TypePath type = arrayType.GetArrayType();
                
                bool validType = valueType.IsCompatibleWith(type, c.Runtime, false);

                if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, type)))
                {
                    env.Errors.Add(new Error(ErrorProvider.ArrayTypeMismatch(arrayType.GetArrayType()), errorToken.Point));
                    return;
                }

                //CompileExpression(env, c, assign.Value);
                valueCompiler();

                if (!validType)
                {
                    env.Code.Add(TypeEvaluator.CastMap[(valueType, type)]);
                }

                CompileExpression(env, c, idx.Target);
                CompileExpression(env, c, idx.Index);
                env.Code.Add("popidx");
            }
        }
        else
        {
            env.Errors.Add(new Error(ErrorProvider.NotAssignableExpression(), errorToken.Point));
        }
    }

    private void CompilePath(CompileEnvironment env, CompileContext c, TypePath type)
    {
        Expression expr = new IdentExpression(type.ModuleName, default);
        for (int i = 1; i < type.Route.Length; i++)
        {
            expr = new DotExpression(expr, type.Route[i], default);
        }
        expr = new DotExpression(expr, type.Name, default);

        void extract(Expression e)
        {
            if (e is IdentExpression id)
            {
                env.Code.Add("mov");
                env.Code.Add(id.Identifier);
            }
            else if (e is DotExpression dot)
            {
                extract(dot.Source);
                env.Code.Add("sel");
                env.Code.Add(dot.Right);
            }
        }

        extract(expr);
    }

    private void CompileFuncSymbol(CompileEnvironment env, CompileContext c, Expression symbolExpr, FuncExpression funcExpr)
    {
        if (symbolExpr is IdentExpression ident)
        {
            string interpolate = c.Runtime.InterpolateGlblFuncModule(ident.Identifier);
            if (c.TryGetLocal(ident.Identifier, out Local lcl))
            {
                // ident is a local.
                env.Code.Add("push");
                env.Code.Add(lcl.Address.ToString());
            }
            else if (c.HasClass && !c.Static && c.ScopeClass.HasFunction(ident.Identifier))
            {
                FunctionBinary func = c.ScopeClass.MatchFunction(ident.Identifier, funcExpr, c, c.ScopeClass.Path);
                if (func is not null)
                {
                    env.Code.Add("self");
                    env.Code.Add("sel");
                    env.Code.Add(ident.Identifier);
                }
            }
            else if (c.HasClass && c.ScopeClass.HasField(ident.Identifier))
            {
                env.Code.Add("self");
                env.Code.Add("heap");
                env.Code.Add(c.ScopeClass.GetFieldIndex(ident.Identifier).ToString());
            }
            else if (c.HasClass && !c.Static && c.ScopeClass.HasStcField(ident.Identifier))
            {
                CompilePath(env, c, c.ScopeClass.Path);
                env.Code.Add("hpstc");
                env.Code.Add(c.ScopeClass.GetStcFieldIndex(ident.Identifier).ToString());
            }
            else if (interpolate is not null)
            {
                env.Code.Add("mov");
                env.Code.Add(interpolate);
                env.Code.Add("sel");
                env.Code.Add(ident.Identifier);
            }
            else
            {
                if (c.Runtime.ModuleExists(ident.Identifier))
                {
                    env.Code.Add("mov");
                    env.Code.Add(ident.Identifier);
                }
                else
                {
                    // try to interpolate;
                    TypePath type = c.Runtime.Interpolate(new TypePath(Array.Empty<string>(), ident.Identifier));
                    if (type is not null)
                    {
                        CompilePath(env, c, type);
                    }
                }
            }
        }
        else if (symbolExpr is DotExpression dot)
        {
            CompileFuncSymbol(env, c, dot.Source, funcExpr);
            TypePath sourceType = TypeEvaluator.Evaluate(dot.Source, c.Runtime, c);
            TypePath pureType = c.Runtime.Interpolate(TypePath.FromModel(new TypeData(dot.Source)));
            if (c.Runtime.TypeExists(sourceType) &&
                c.Runtime.IsClass(sourceType))
            {
                ClassBinary cls = c.Runtime.GetClass(sourceType);

                if (!c.IsClassAccessible(cls))
                {
                    env.Errors.Add(new Error(ErrorProvider.NotAccessible(cls.Path.ToString()), dot.Source.Token.Point));
                }

                if (cls.HasField(dot.Right))
                {
                    env.Code.Add("heap");
                    env.Code.Add(cls.GetFieldIndex(dot.Right).ToString());
                }
                else if (cls.HasFunction(dot.Right))
                {
                    FunctionBinary f = cls.MatchFunction(dot.Right, funcExpr, c, sourceType);
                    if (f is not null)
                    {
                        // if (!c.IsMethodAccessible(cls, f))
                        // {
                        //     env.Errors.Add(new Error(ErrorProvider.NotAccessible(f.Name), dot.Token.Point));
                        // }
                        

                        env.Code.Add("sel");
                        env.Code.Add(f.Name);
                    }
                }
                else
                {
                    env.Errors.Add(new Error(ErrorProvider.ClassSymbolNotFound(dot.Right, sourceType), dot.Source.Token.Point));
                }
            }
            else if (pureType is not null && c.Runtime.TypeExists(pureType) &&
                    c.Runtime.IsClass(pureType))
            {
                ClassBinary cls = c.Runtime.GetClass(pureType);

                if (!c.IsClassAccessible(cls))
                {
                    env.Errors.Add(new Error(ErrorProvider.NotAccessible(cls.Path.ToString()), dot.Source.Token.Point));
                }

                if (cls.HasStcField(dot.Right))
                {
                    FieldBinary stc = cls.GetStcField(dot.Right);

                    if (!c.IsFieldAccessible(cls, stc))
                    {
                        env.Errors.Add(new Error(ErrorProvider.NotAccessible(stc.Name), dot.Token.Point));
                    }

                    CompilePath(env, c, pureType);
                    env.Code.Add("hpstc");
                    env.Code.Add(cls.GetStcFieldIndex(dot.Right).ToString());
                }
                else if (cls.HasStcFunction(dot.Right))
                {
                    FunctionBinary f = cls.MatchStcFunction(dot.Right, funcExpr, c);

                    // if (!c.IsMethodAccessible(cls, f))
                    // {
                    //     env.Errors.Add(new Error(ErrorProvider.NotAccessible(f.Name), dot.Token.Point));
                    // }

                    if (f is not null)
                    {
                        env.Code.Add("sel");
                        env.Code.Add(f.Name);
                    }
                }
            }
            else
            {
                env.Code.Add("sel");
                env.Code.Add(dot.Right);
            }
        }
        else if (symbolExpr is IndexExpression idx)
        {
            CompileFuncSymbol(env, c, idx.Target, funcExpr);
            CompileExpression(env, c, idx.Index);
            env.Code.Add("idx");
        }
        else
        {
            CompileExpression(env, c, symbolExpr);
        }
    }

    private void CompileExpression(CompileEnvironment env, CompileContext c, Expression expression)
    {
        if (expression is ImmediateExpression imm)
        {
            if (imm.Type == BuiltinTypes.STRING)
            {
                env.Code.Add("pushstr");
                env.Code.Add(imm.String);
            }
            else if (imm.Type == BuiltinTypes.INT)
            {
                env.Code.Add("pushint");
                env.Code.Add(imm.Int.ToString());
            }
            else if (imm.Type == BuiltinTypes.FLOAT)
            {
                env.Code.Add("pushflt");
                env.Code.Add(imm.Float.ToString());
            }
            else if (imm.Type == BuiltinTypes.DOUBLE)
            {
                env.Code.Add("pushdbl");
                env.Code.Add(imm.Float.ToString());
            }
            else if (imm.Type == BuiltinTypes.BOOLEAN)
            {
                env.Code.Add("pushbool");
                env.Code.Add(imm.Boolean ? "true" : "false");
            }
        }
        else if (expression is SelfExpression)
        {
            env.Code.Add("self");
        }
        else if (expression is NullExpression)
        {
            env.Code.Add("pushnull");
        }
        else if (expression is NvRetvExpression)
        {
            env.Code.Add("nvretv");
        }
        else if (expression is CalcExpression calc)
        {
            TypePath leftType = TypeEvaluator.Evaluate(calc.Left, c.Runtime, c);
            TypePath rightType = TypeEvaluator.Evaluate(calc.Right, c.Runtime, c);
            if (calc.Operator != Operator.Equal && calc.Operator != Operator.NotEqual &&
                !TypeEvaluator.TypeMap.ContainsKey((leftType, rightType, calc.Operator)) &&
                !TypeEvaluator.TypeMap.ContainsKey((rightType, leftType, calc.Operator)))
            {
                env.Errors.Add(new Error(ErrorProvider.OperatorTypeMismatch(leftType, rightType, calc.Operator), calc.Left.Token.Point));
            }

            CompileExpression(env, c, calc.Left);
            CompileExpression(env, c, calc.Right);
            env.Code.Add(CalcMap[(int)calc.Operator]);
        }
        else if (expression is IdentExpression ident)
        {
            string name = ident.Identifier;

            if (c.TryGetLocal(name, out Local lcl))
            {
                env.Code.Add("push");
                env.Code.Add(lcl.Address.ToString());
            }
            else if (c.HasClass && c.ScopeClass.HasField(name))
            {
                ClassBinary cls = c.ScopeClass;
                int idx = cls.GetFieldIndex(name);
                env.Code.Add("self");
                env.Code.Add("heap");
                env.Code.Add(idx.ToString());
            }
            else if (c.HasClass && c.ScopeClass.HasStcField(name))
            {
                ClassBinary cls = c.ScopeClass;
                env.Code.Add("hpstc");
                env.Code.Add(cls.GetStcFieldIndex(name).ToString());
            }
            else if (c.Runtime.ModuleExists(name) ||
                    c.Runtime.Interpolate(new TypePath(Array.Empty<string>(), name)) is not null)
            {
            }
            else
            {
                env.Errors.Add(new Error(ErrorProvider.SymbolNotFoundInScope(name), ident.Token.Point));
            }
        }
        else if (expression is FuncExpression func)
        {
            // process overloading.
            
            (FunctionBinary function, ClassBinary cb, TypePath type, Expression source) = TypeEvaluator.FindFunctionWithClassPathSource(func, c.Runtime, c);

            bool valid = TypeEvaluator.IsValidNameFunction(func.Function, c.Runtime, c);

            if (function is null)
            {
                // invalid function.
                if (valid)
                {
                    // overloading error.
                    env.Errors.Add(new Error(ErrorProvider.InvalidFunctionOverload(), func.Function.Token.Point));
                }
                else
                {
                    // invalid function.
                    env.Errors.Add(new Error(ErrorProvider.InvalidFunction(), func.Function.Token.Point));
                }
                return;
            }

            if (function is not null && cb is not null &&
            !c.IsMethodAccessible(cb, function))
            {
                env.Errors.Add(new Error(ErrorProvider.NotAccessible(function.Name), func.Function.Token.Point));
            }

            if (function.IsExtendR)
            {
                // 最初にセルフ引数を積む。
                CompileExpression(env, c, source);
            }

            
            CompileFuncArguments(env, c, function, func.Arguments, func.Function.Token, type: type, cb: cb);
            

            if (!function.IsExtendR)
            {
                CompileFuncSymbol(env, c, func.Function, func);
                // FIXME: 
                env.Code[^1] += function.Name.Replace(env.Code[^1], string.Empty);
            }
            else
            {
                env.Code.Add("mov");
                env.Code.Add(function.Module);
                env.Code.Add("sel");
                env.Code.Add(function.Name);
            }
            env.Code.Add("call");
        }
        else if (expression is DotExpression dot)
        {
            TypePath sourceType = TypeEvaluator.Evaluate(dot.Source, c.Runtime, c);
            TypePath pureType = c.Runtime.Interpolate(TypePath.FromModel(new TypeData(dot.Source)));
            CompileExpression(env, c, dot.Source);

            if (sourceType.NotNull() && c.Runtime.TypeExists(sourceType) &&
                c.Runtime.IsClass(sourceType))
            {
                ClassBinary cls = c.Runtime.GetClass(sourceType);

                if (!c.IsClassAccessible(cls))
                {
                    env.Errors.Add(new Error(ErrorProvider.NotAccessible(cls.Path.ToString()), dot.Source.Token.Point));
                }

                if (cls.HasField(dot.Right))
                {
                    FieldBinary field = cls.GetField(dot.Right);
                    if (!c.IsFieldAccessible(cls, field))
                    {
                        env.Errors.Add(new Error(ErrorProvider.NotAccessible(field.Name), dot.Token.Point));
                    }

                    int index = cls.GetFieldIndex(dot.Right);
                    env.Code.Add("heap");
                    env.Code.Add(index.ToString());
                }
                else
                {
                    env.Errors.Add(new Error(ErrorProvider.ClassSymbolNotFound(dot.Right, sourceType), dot.Token.Point));
                }
            }
            else if (pureType is not null && c.Runtime.TypeExists(pureType) &&
                    c.Runtime.IsClass(pureType))
            {
                ClassBinary cls = c.Runtime.GetClass(pureType);

                if (!c.IsClassAccessible(cls))
                {
                    env.Errors.Add(new Error(ErrorProvider.NotAccessible(cls.Path.ToString()), dot.Source.Token.Point));
                }

                if (cls.HasStcField(dot.Right))
                {
                    FieldBinary stc = cls.GetStcField(dot.Right);

                    if (!c.IsFieldAccessible(cls, stc))
                    {
                        env.Errors.Add(new Error(ErrorProvider.NotAccessible(stc.Name), dot.Token.Point));
                    }

                    CompilePath(env, c, pureType);
                    env.Code.Add("hpstc");
                    env.Code.Add(cls.GetStcFieldIndex(dot.Right).ToString());
                }
            }
        }
        else if (expression is DecrExpression decr)
        {
            TypePath targetType = TypeEvaluator.Evaluate(decr.Target, c.Runtime, c);
            if (!TypeEvaluator.Incrementable.Contains(targetType))
            {
                env.Errors.Add(new Error(ErrorProvider.SingleOperatorTypeMismatch(targetType, "--"), decr.Target.Token.Point));
            }

            CompileExpression(env, c, decr.Target);
            env.Code.Add("pushint");
            env.Code.Add("1");
            env.Code.Add("sub");
        }
        else if (expression is IncrExpression incr)
        {
            TypePath targetType = TypeEvaluator.Evaluate(incr.Target, c.Runtime, c);
            if (!TypeEvaluator.Incrementable.Contains(targetType))
            {
                env.Errors.Add(new Error(ErrorProvider.SingleOperatorTypeMismatch(targetType, "++"), incr.Target.Token.Point));
            }

            CompileExpression(env, c, incr.Target);
            env.Code.Add("pushint");
            env.Code.Add("1");
            env.Code.Add("add");
        }
        else if (expression is IndexExpression idx)
        {
            TypePath targetType = TypeEvaluator.Evaluate(idx.Target, c.Runtime, c);
            if (!targetType.IsArray)
            {
                env.Errors.Add(new Error(ErrorProvider.IndexTargetNotArray(), idx.Target.Token.Point));
            }

            CompileExpression(env, c, idx.Target);
            CompileExpression(env, c, idx.Index);
            env.Code.Add("idx");
        }
        else if (expression is CastExpression cast)
        {
            TypePath fromType = TypeEvaluator.Evaluate(cast.Value, c.Runtime, c);
            TypePath toType = TypePath.FromModel(cast.TargetType);

            if (TypeEvaluator.CastMap.TryGetValue((fromType, toType), out string inst))
            {
                CompileExpression(env, c, cast.Value);
                env.Code.Add(inst);
            }
            else
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidPrimitiveCast(fromType, toType), cast.Token.Point));
            }
        }
        else if (expression is ObjectCastExpression oct)
        {
            TypePath fromType = TypeEvaluator.Evaluate(oct.Source, c.Runtime, c);
            TypePath toType = c.Runtime.Interpolate(TypePath.FromModel(oct.TargetType));

            if (!c.Runtime.TypeExists(toType))
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidType(toType), oct.Token.Point));
            }

            if (!toType.IsCompatibleWith(toType, c.Runtime))
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidPrimitiveCast(fromType, toType), oct.Token.Point));
            }

            CompileExpression(env, c, oct.Source);
            env.Code.Add("cstobj");
            env.Code.Add(toType.ToString());
        }
        else if (expression is FuncPointerExpression ptr)
        {
            bool valid = TypeEvaluator.IsValidNameFunction(ptr.TargetFunction, c.Runtime, c);
            if (!valid)
            {
                // error.
                env.Errors.Add(new Error(ErrorProvider.InvalidFunction(), ptr.TargetFunction.Token.Point));
                return;
            }

            // TODO: pass the arg types.
            FuncExpression funcExpr = new FuncExpression(ptr.TargetFunction, ptr.ArgumentTypes.Select(x => new DummyExpression(x)).Cast<Expression>().ToList());
            (FunctionBinary function, ClassBinary cb, TypePath type, Expression src) = TypeEvaluator.FindFunctionWithClassPathSource(funcExpr, c.Runtime, c);
            
            if (function is null)
            {
                env.Errors.Add(new Error(ErrorProvider.InvalidFunctionOverload(), ptr.TargetFunction.Token.Point));
                return;
            }
            
            CompileFuncSymbol(env, c, ptr.TargetFunction, funcExpr);
            env.Code[^1] += function.Name.Remove(0, function.Name.IndexOf('~'));
            env.Code.Add("fptr");
        }
        else if (expression is AllocExpression alloc)
        {
            TypePath lengthType = TypeEvaluator.Evaluate(alloc.Length, c.Runtime, c);
            TypePath arrayType = c.Runtime.Interpolate(TypePath.FromModel(alloc.ArrayType));
            if (!lengthType.IsCompatibleWith(TypePath.INT, c.Runtime))
            {
                // invalid length type.
                env.Errors.Add(new Error(ErrorProvider.ArrayLengthNotInt(), alloc.Token.Point));
            }

            if (c.Runtime.IsClass(arrayType))
            {
                ClassBinary cls = c.Runtime.GetClass(arrayType);
                if (!c.IsClassAccessible(cls))
                {
                    env.Errors.Add(new Error(ErrorProvider.NotAccessible(cls.Path.ToString()), alloc.Token.Point));
                }
            }

            CompileExpression(env, c, alloc.Length);
            env.Code.Add("arr");
            env.Code.Add(arrayType.ToString());
        }
        else if (expression is NewExpression ne)
        {
            TypePath targetType = c.Runtime.Interpolate(TypePath.FromModel(ne.Type));
            if (c.Runtime.TypeExists(targetType) &&
                c.Runtime.IsClass(targetType))
            {
                ClassBinary cls = c.Runtime.GetClass(targetType);

                if (!c.IsClassAccessible(cls))
                {
                    env.Errors.Add(new Error(ErrorProvider.NotAccessible(cls.Path.ToString()), ne.Token.Point));
                }

                if (cls.HasFunction("ctor"))
                {
                    // with constructor.
                    FunctionBinary constructor = cls.MatchFunction("ctor", new FuncExpression(null, ne.Arguments), c, targetType);
                    if (constructor is not null)
                    {
                        if (!c.IsMethodAccessible(cls, constructor))
                        {
                            env.Errors.Add(new Error(ErrorProvider.ConstructorNotAccessible(), ne.Token.Point));
                        }


                        // args

                        CompileFuncArguments(env, c, constructor, ne.Arguments, ne.Token, type: targetType, cb: cls);


                        env.Code.Add("inst");
                        env.Code.Add(targetType.ToString());
                        env.Code.Add("pushs");
                        env.Code.Add("sel");
                        env.Code.Add(constructor.Name);
                        env.Code.Add("call");
                        env.Code.Add("pops");
                    }
                    else
                    {
                        env.Errors.Add(new Error(ErrorProvider.InvalidConstructor(), ne.Token.Point));
                    }
                }
                else
                {
                    // without constructor.
                    env.Code.Add("inst");
                    env.Code.Add(cls.Path.ToString());
                }
            }
            else
            {
                env.Errors.Add(new Error(ErrorProvider.NotInstantiatableType(targetType), ne.Token.Point));
            }
        }
    }

    private void CompileFuncArguments(CompileEnvironment env, CompileContext c, FunctionBinary function, List<Expression> args, Token repToken, TypePath type = null, ClassBinary cb = null)
    {
        for (int i = 0; i < args.Count; i++)
        {
            // type check:
            TypePath valueType = TypeEvaluator.Evaluate(args[i], c.Runtime, c);
            TypePath argType = function.Arguments[i].Type;
            
            if (cb is not null && cb.GenericCount > 0 &&
                type.Generics.Count > 0)
            {
                argType = c.Runtime.GetWithGeneric(type, argType);
            }

            bool validType = true;
            if (function is not null)
            {
                // type check
                validType = valueType.IsCompatibleWith(argType, c.Runtime, false);
                if (!validType && !TypeEvaluator.ImplicitCastMap.Contains((valueType, argType)))
                {
                    env.Errors.Add(new Error(ErrorProvider.ArgumentTypeMismatch(i, argType), repToken.Point));
                    return;
                }
            }

            
            //

            CompileExpression(env, c, args[i]);

            if (!validType)
            {
                env.Code.Add(TypeEvaluator.CastMap[(valueType, argType)]);
            }
        }
    }
}