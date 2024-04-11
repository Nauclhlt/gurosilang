namespace Gurosi;

public sealed class LocalContext
{
    private ProgramRuntime _runtime;
    private RuntimeStack _mainStack;
    private RuntimeStack _tempStack;
    private SlicedMemory _memory;
    private CompiledCode _code;

    private int _counter = 0;

    public ProgramRuntime Runtime => _runtime;
    public SlicedMemory Memory => _memory;
    public CompiledCode Code => _code;

    public LocalContext(ProgramRuntime runtime, SlicedMemory memory, CompiledCode code)
    {
        _runtime = runtime;
        _memory = memory;
        _code = code;
        
        _mainStack = runtime.MainStack;
        _tempStack = runtime.TempStack;
    }

    public bool RunInstruction()
    {
        if (_code.EndOfCode)
            return true;

        byte opcode = _code.ReadOpCode();

        switch (opcode)
        {
            case GIL.PUSH:
                {
                    // ��������̒l�����C���X�^�b�N�ɐςށB
                    int addr = _code.ReadAddress();
                    IValueObject source = _memory.Read(addr);
                    _mainStack.Push(source);
                    break;
                }
            case GIL.POP:
                {
                    // ���C���X�^�b�N����l���Ƃ肾���A��������ɓ����B
                    int addr = _code.ReadAddress();
                    IValueObject source = _mainStack.Pop();
                    _memory.Write(addr, source);
                    break;
                }
            case GIL.DISC:
                {
                    // ���C���X�^�b�N����l���Ƃ肾���A�̂Ă�B
                    _mainStack.Pop();
                    break;
                }
            case GIL.PUSHM:
                {
                    // ��������̒l���ꎞ�X�^�b�N�ɐςށB
                    int addr = _code.ReadInt();
                    IValueObject source = _memory.Read(addr);
                    _tempStack.Push(source);
                    break;
                }
            case GIL.PUSHS:
                {
                    // �X�^�b�N��̒l���ꎞ�X�^�b�N�ɐςށB
                    IValueObject source = _mainStack.Top();
                    _tempStack.Push(source);
                    break;
                }
            case GIL.POPM:
                {
                    // �ꎞ�X�^�b�N����l���Ƃ肾���A��������ɓ����B
                    int addr = _code.ReadInt();
                    IValueObject source = _tempStack.Pop();
                    _memory.Write(addr, source);
                    break;
                }
            case GIL.POPS:
                {
                    // �ꎞ�X�^�b�N����l���Ƃ肾���A���C���X�^�b�N�ɐςށB
                    IValueObject source = _tempStack.Pop();
                    _mainStack.Push(source);
                    break;
                }
            case GIL.POPF:
                {
                    // ���C���X�^�b�N�ォ��l���Ƃ肾���A���̉��ɐς܂ꂽ�N���X�̃C���X�^���X�ɂ�����i�Ԗڂ̃t�B�[���h�Ɋi�[����B

                    break;
                }
            case GIL.POPIDX:
                {
                    // ���C���X�^�b�N�ォ��l���Ƃ肾���A���̉��ɐς܂ꂽ�z��̎w��C���f�b�N�X�Ɋi�[����B
                    // �X�^�b�N�ォ�� �C���f�b�N�X => �z�� => �l�B
                    break;
                }
            case GIL.POPSTC:
                {
                    // ���C���X�^�b�N�ォ��l���Ƃ肾���A���̉��ɐς܂ꂽ���O�ɂ�����i�Ԗڂ̐ÓI�ϐ��ɒl���i�[����B
                    break;
                }
            case GIL.ADD:
                {
                    // ���C���X�^�b�N��̒l�����Z����B�X�^�b�N�̏ォ��(�E��)=>(����)
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();
                    // �v�Z�B�^�̓R���p�C�����ɕۏ؂���Ă���B

                    break;
                }
            case GIL.SUB:
                {
                    break;
                }
            case GIL.MUL:
                {
                    break;
                }
            case GIL.DIV:
                {
                    break;
                }
            case GIL.MOD:
                {
                    break;
                }
            case GIL.LT:
                {
                    break;
                }
            case GIL.LTE:
                {
                    break;
                }
            case GIL.GT:
                {
                    break;
                }
            case GIL.GTE:
                {
                    break;
                }
            case GIL.EQ:
                {
                    break;
                }
            case GIL.NEQ:
                {
                    break;
                }
            case GIL.AND:
                {
                    break;
                }
            case GIL.OR:
                {
                    break;
                }
            case GIL.XOR:
                {
                    break;
                }
            case GIL.COMP:
                {
                    break;
                }
            case GIL.SHL:
                {
                    break;
                }
            case GIL.SHR:
                {
                    break;
                }
            case GIL.JMP:
                {
                    // ���߂𑊑ΓI�ɃW�����v����B�p�����[�^�̓I�t�Z�b�g�l�B
                    break;
                }
            case GIL.CJMP:
                {
                    // �X�^�b�N��̒l��true�Ȃ�W�����v����B
                    break;
                }
            case GIL.NJMP:
                {
                    // false�Ȃ�B
                    break;
                }
            case GIL.TFJMP:
                {
                    // true�Ȃ���p�����[�^, false�Ȃ���p�����[�^�W�����v����B
                    break;
                }
            case GIL.PJMP:
                {
                    // �X�^�b�N��̒l�����Ȃ�W�����v����B
                    break;
                }
            case GIL.MJMP:
                {
                    // �X�^�b�N��̒l�����Ȃ�W�����v����B
                    break;
                }
            case GIL.ZJMP:
                {
                    // �X�^�b�N��̒l���[���Ȃ�W�����v����B
                    break;
                }
            case GIL.SEL:
                {
                    // �X�^�b�N��̒l����w�肵�����O��I������B
                    break;
                }
            case GIL.ALLOC:
                {
                    // �w�肵���A�h���X���w�肵���^�Ŋm�ۂ���B
                    break;
                }
            case GIL.ARR:
                {
                    // �w�肵���^�̔z����쐬���āA�X�^�b�N��ɐςށB
                    // �X�^�b�N��̒l�𒷂��Ƃ���B
                    break;
                }
            case GIL.INST:
                {
                    // �w�肵���N���X�̃C���X�^���X���쐬����B
                    break;
                }
            case GIL.RET:
                {
                    // �X�^�b�N��̒l�����o���A�Ăяo�����ɕԋp����B
                    break;
                }
            case GIL.ERR:
                {
                    // ���s���G���[���g���K�[����B
                    // �X�^�b�N�ォ�� �G���[���b�Z�[�W => �G���[�R�[�h
                    break;
                }
            // �ȉ����l��push
            case GIL.PUSHSTR:
                {
                    break;
                }
            case GIL.PUSHINT:
                {
                    break;
                }
            case GIL.PUSHFLT:
                {
                    break;
                }
            case GIL.PUSHDBL:
                {
                    break;
                }
            case GIL.PUSHBOOL:
                {
                    break;
                }
            case GIL.PUSHNULL:
                {
                    break;
                }
            case GIL.MOV:
                {
                    // �w�肵�����O�̃I�u�W�F�N�g���X�^�b�N�ɐςށB
                    // (�֐��A�N���X�A�ϐ��A�t�B�[���h��)
                    break;
                }
            case GIL.CALL:
                {
                    // �X�^�b�N�ɐς܂ꂽ�֐����Ăяo���B
                    break;
                }
            case GIL.IDX:
                {
                    // �X�^�b�N�ɐς܂ꂽ�l���C���f�b�N�X�Ƃ��Ēl�����o���A�X�^�b�N�ɐςށB
                    // �X�^�b�N��ŏォ��C���f�b�N�X�A���o�����B
                    break;
                }
            case GIL.SET:
                {
                    // This may not be used.
                    break;
                }
            case GIL.SELF:
                {
                    // ���ݎ��s����Ă���N���X�̃C���X�^���X���X�^�b�N��ɐςށB
                    break;
                }
            case GIL.CPL:
                {
                    // continue�ňړ����郉�x���B
                    break;
                }
            case GIL.BPL:
                {
                    // break�ňړ����郉�x���B
                    break;
                }
            case GIL.CONT:
                {
                    // continue
                    break;
                }
            case GIL.BRK:
                {
                    // break
                    break;
                }
            case GIL.HEAP:
                {
                    // �X�^�b�N��̃N���X�̃C���X�^���X�ɂ�����i�Ԗڂ̃t�B�[���h���X�^�b�N�ɐςށB
                    break;
                }
            case GIL.HPSTC:
                {
                    // �X�^�b�N��̃N���X�ɂ�����i�Ԗڂ̐ÓI�t�B�[���h���X�^�b�N�ɐςށB
                    break;
                }
            case GIL.NVC:
                {
                    // �����^�C�������֐��������Ăяo���B
                    break;
                }
            case GIL.NVRETV:
                {
                    // ���C���^�C�������֐�����Ԃ��ꂽ�l���X�^�b�N��ɐςށB
                    break;
                }
            case GIL.CSTIF:
                {
                    // int => float �̃L���X�g���s���B
                    break;
                }
            case GIL.CSTID:
                {
                    // int => double �̃L���X�g���s���B
                    break;
                }
            case GIL.CSTFD:
                {
                    // float => double �̃L���X�g���s���B
                    break;
                }
            case GIL.INC:
                {
                    // �C���N�������g
                    break;
                }
            case GIL.DEC:
                {
                    // �f�N�������g
                    break;
                }
            case GIL.CSTOBJ:
                {
                    // �X�^�b�N��̃I�u�W�F�N�g��ړI�̌^�ɃL���X�g����B
                    break;
                }
            case GIL.FPTR:
                {
                    // �X�^�b�N��̊֐��̃|�C���^���擾���A�X�^�b�N�ɐςށB
                    break;
                }
            default:
                // unknown instruction.
                return true;
        }

        return false;
    }
}