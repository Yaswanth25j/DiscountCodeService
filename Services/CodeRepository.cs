using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using DiscountCodeService.Models;

namespace DiscountCodeService.Services
{
    public class CodeRepository
    {
        private const string StorageFile = "discountcodes.json";
        private readonly ConcurrentDictionary<string, DiscountCode> _codes;

        public CodeRepository()
        {
            if (File.Exists(StorageFile))
            {
                var json = File.ReadAllText(StorageFile);
                _codes = JsonSerializer.Deserialize<ConcurrentDictionary<string, DiscountCode>>(json) ?? new();
            }
            else
            {
                _codes = new();
            }
        }

        public bool AddCodes(int count, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[length];
            int added = 0;

            for (int i = 0; i < count; i++)
            {
                string code;
                do
                {
                    rng.GetBytes(buffer);
                    code = new string(buffer.Select(b => chars[b % chars.Length]).ToArray());
                } while (_codes.ContainsKey(code));

                if (_codes.TryAdd(code, new DiscountCode { Code = code, Used = false }))
                    added++;
            }

            Save();
            return added == count;
        }

        public byte UseCode(string code)
        {
            if (!_codes.TryGetValue(code, out var entry)) return 2;
            if (entry.Used) return 1;

            entry.Used = true;
            Save();
            return 0;
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(_codes);
            File.WriteAllText(StorageFile, json);
        }
    }
}