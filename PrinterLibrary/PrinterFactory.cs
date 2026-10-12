using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrinterLibrary;

public class PrinterFactory
{
    public IPrint GetPrinter() => new Epson();
}
