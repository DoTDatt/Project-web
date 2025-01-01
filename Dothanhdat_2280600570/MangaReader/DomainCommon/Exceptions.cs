using System;

namespace MangaReader.DomainCommon;

public class NetworkException : Exception
{
    public NetworkException(string msg) : base(msg)
    {
        
    }
}

public class ParseException : Exception
{
    
}