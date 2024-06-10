namespace gbemu;

// refer to https://github.com/davidwhitney/CoreBoy/blob/master/CoreBoy/AddressSpace.cs
public interface IAddressSpace
{
    bool Accepts(ushort address);
    void SetByte(ushort address, byte value);
    byte GetByte(ushort address);
}