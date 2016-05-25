using System;
using System.IO;


namespace PocketButler.Helpers.Media
{
    
    public interface IMediaFile : IDisposable {

        string Path { get; }
        Stream GetStream();
    }
}
