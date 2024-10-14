using gbemu.cart;

namespace gbemu.bus;

// 0x0000 - 0x3FFF : ROM Bank 0
// 0x4000 - 0x7FFF : ROM Bank 1 - Switchable
// 0x8000 - 0x97FF : CHR RAM
// 0x9800 - 0x9BFF : BG Map 1
// 0x9C00 - 0x9FFF : BG Map 2
// 0xA000 - 0xBFFF : Cartridge RAM
// 0xC000 - 0xCFFF : RAM Bank 0
// 0xD000 - 0xDFFF : RAM Bank 1-7 - switchable - Color only
// 0xE000 - 0xFDFF : Reserved - Echo RAM
// 0xFE00 - 0xFE9F : Object Attribute Memory
// 0xFEA0 - 0xFEFF : Reserved - Unusable
// 0xFF00 - 0xFF7F : I/O Registers
// 0xFF80 - 0xFFFE : Zero Page

public class Bus
{
    private readonly List<IAddressSpace> _addressSpace = new List<IAddressSpace>();

    public void AddSpace(IAddressSpace space) => _addressSpace.Add(space);

    public byte Read(ushort address)
    {
        foreach (var cSpace in _addressSpace)
        {
            if (cSpace.Accepts(address))
                return cSpace.GetByte(address);
        }
        throw new ArgumentException($"Invalid address: {address}");
    }

    public void Write(ushort address, byte value)
    {
        foreach (var cSpace in _addressSpace)
        {
            if (cSpace.Accepts(address))
                cSpace.SetByte(address, value);
        }
        throw new ArgumentException($"Invalid address: {address}");
    }

}