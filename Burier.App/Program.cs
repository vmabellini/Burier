using Burier.App.Features;
using Mono.Options;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Burier.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new Controller(args);
            controller.Execute();
        }
    }
}
