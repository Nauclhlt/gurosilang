namespace Gurosi;

public interface IBinary
{
    public void Read(BinaryReader reader);
    public void Write(BinaryWriter writer);
}