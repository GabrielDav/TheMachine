using System.IO;
using System.IO.IsolatedStorage;

namespace Engine.Core
{

    public class StorageControl
    {
#if WINDOWS_PHONE

        protected IsolatedStorageFile _storage;
        protected IsolatedStorageFileStream _stream;

        public StorageControl()
        {
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public bool DirectoryExists(string path)
        {
            return _storage.DirectoryExists(path);
        }

        public void CreateDirecotry(string dir)
        {
            _storage.CreateDirectory(dir);
        }

        public bool FileExists(string path)
        {
            return _storage.FileExists(path);
        }

        public BinaryReader OpenFileBinaryRead(string path)
        {
            if (_stream != null)
                throw new IOException("Cannot open file '" + path + "' stream is not closed.");
            _stream = _storage.OpenFile(path, FileMode.Open);
            return new BinaryReader(_stream);

        }

        public BinaryWriter OpenFileBinaryWrite(string path)
        {
            if (_stream != null)
                throw new IOException("Cannot open file '" + path + "' stream is not closed.");
            _stream = _storage.OpenFile(path, FileMode.Create);
            return new BinaryWriter(_stream);
        }

        public void CloseStream()
        {
            _stream.Close();
            _stream = null;
        }

#elif WINDOWS || EDITOR

        protected readonly string _globalPath;
        protected FileStream _stream;

        public StorageControl()
        {
            _globalPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
            if (!Directory.Exists("Storage"))
                Directory.CreateDirectory("Storage");
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(Path.Combine(_globalPath, path));
        }

        public void CreateDirecotry(string dir)
        {
            Directory.CreateDirectory(Path.Combine(_globalPath,dir));
        }

        public bool FileExists(string path)
        {
            return File.Exists(Path.Combine(_globalPath, path));
        }

        public BinaryReader OpenFileBinaryRead(string path)
        {
            if (_stream != null)
                throw new IOException("Cannot open file '" + path + "' stream is not closed.");
            _stream = File.Open(Path.Combine(_globalPath, path), FileMode.Open);
            return new BinaryReader(_stream);

        }

        public BinaryWriter OpenFileBinaryWrite(string path)
        {
            if (_stream != null)
                throw new IOException("Cannot open file '" + path + "' stream is not closed.");
            _stream = File.Open(Path.Combine(_globalPath, path), FileMode.Create);
            return new BinaryWriter(_stream);
        }

        public void CloseStream()
        {
            _stream.Close();
            _stream = null;
        }

#endif
    }
}
