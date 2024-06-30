using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Exceptions;
public class SignatureException : Exception
{
    public SignatureException(byte[] bytes) 
        : base("the signature of this input stream does not have a valid starting sequence of bytes.")
    {
        Bytes = bytes;
    }

    public readonly byte[] Bytes;
}
