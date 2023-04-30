using System.Threading.Tasks;

namespace Database
{
    public class Load
    {
        public static bool DataBaseLoaded = false;

        public static async Task Database()
        {
            DataBaseLoaded = false;
            await Task.Run(() => { Items.Load.Json(); });
            await Task.Run(() => { Blessings.Load.Json(); });
            await Task.Run(() => { Affix.Load.Prefixs(); });
            await Task.Run(() => { Affix.Load.Suffixs(); });
            await Task.Run(() => { Materials.Glyphs.Load.Json(); });
            await Task.Run(() => { Materials.Runes.Load.Json(); });
            await Task.Run(() => { Items.Keys.Load.Json(); });
            await Task.Run(() => { Skills.Load.Json(); });
            
            DataBaseLoaded = true;
        }
    }
}
