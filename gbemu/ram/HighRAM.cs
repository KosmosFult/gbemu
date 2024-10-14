namespace gbemu.ram;

public class HighRAM : IAddressSpace
{

    private const ushort LowerBound = 0xFF80;
    private const ushort UpperBound = 0xFFFE;
    
    public bool Accepts(ushort address) 
        => address is >= LowerBound and <= UpperBound; 

    public void SetByte(ushort address, byte value)
    {
        _data[address - LowerBound] = value;
    }

    public byte GetByte(ushort address)
    {
        return _data[address - LowerBound];
    }

    private byte[] _data = new byte[128];
}