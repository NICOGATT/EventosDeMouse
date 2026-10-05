using System;
using System.Runtime.InteropServices;

class Program
{
    const uint STD_INPUT_HANDLE = unchecked((uint)-10);
    const uint ENABLE_MOUSE_INPUT = 0x0010;
    const uint ENABLE_EXTENDED_FLAGS = 0x0080;
    const uint ENABLE_WINDOWS_INPUT = 0x0008; 
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(uint nStdHandle);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out]
    INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

    [StructLayout(LayoutKind.Explicit)]
    struct INPUT_RECORD
    {
        [FieldOffset(0)]
        public ushort EventType;
        [FieldOffset(4)]
        public MOUSE_EVENT_RECORD MouseEvent;
    }
    struct MOUSE_EVENT_RECORD
    {
        public COORD dwMousePosition;
        public uint dwButtonState;
        public uint dwControlKeyState;
        public uint dwEventFlags;
    }

    struct COORD
    {
        public short X;
        public short Y;
    }
    const ushort MOUSE_EVENT = 0x0002;
    const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;

    static void Main()
    {
        IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
        GetConsoleMode(consoleHandle, out uint consoleMode);

        //Activar la entrada del mouse 
        SetConsoleMode(consoleHandle, consoleMode | ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS | ENABLE_WINDOWS_INPUT);
        Console.WriteLine("Haz click en cualquier parte de la consola(Presiona Ctrl+C para salir):");
        INPUT_RECORD[] record = new INPUT_RECORD[1];
        while (true)
        {
            ReadConsoleInput(consoleHandle, record, 1, out uint eventsRead);
            if (eventsRead <= 0 || record[0].EventType != MOUSE_EVENT)
            {
                continue; 
            }
            //Extremos la logico del mouse a sus propia funcion 
            ManejarEventoMouse(record[0].MouseEvent);
        }
    }

    private static void ManejarEventoMouse(MOUSE_EVENT_RECORD mouseEvent)
    {
        //Clausula de guardia para los flags y el boton del mouse 
        bool esClickIzquierdo = mouseEvent.dwButtonState == FROM_LEFT_1ST_BUTTON_PRESSED;
        if (!esClickIzquierdo)
        {   
            return; 
        }
        //Logica principal cuando si ocurre el click izquierdo 
        Console.SetCursorPosition(0, 2);
        Console.WriteLine($"Click detectado en X: {mouseEvent.dwMousePosition.X}, Y : {mouseEvent.dwMousePosition.Y}");
    }
}
