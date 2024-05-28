using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace BenchMark.BenchMark;

[MemoryDiagnoser]
public class Capitalize
{
    private readonly string stringTest =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum massa magna, lacinia quis quam in, varius imperdiet velit. Maecenas commodo ipsum vel elit ornare, id condimentum ligula varius. Phasellus nec sapien convallis purus suscipit suscipit. Quisque semper nunc et venenatis molestie. Nunc tempor ex a odio consequat, eget suscipit elit finibus. Sed sit amet sagittis magna, a malesuada orci. Fusce porta, arcu a congue faucibus, felis enim imperdiet arcu, eget interdum urna leo id leo. Proin eu pharetra est. Curabitur tristique justo et dolor iaculis consectetur. Sed maximus euismod nisl eu aliquam. Nulla pretium volutpat erat at facilisis. Vivamus condimentum malesuada massa, quis vehicula dui venenatis ut. Nam lobortis sodales lectus et tristique. Ut at mollis dolor.\r\n\r\nMaecenas ultricies, massa consequat pretium vehicula, sem elit posuere quam, ut porttitor risus lorem a tortor. Duis fringilla, diam in accumsan egestas, sapien nisl porta ex, vel efficitur dolor quam at odio. Aliquam erat volutpat. Suspendisse a turpis lorem. Etiam dignissim urna sed ipsum posuere, aliquet commodo arcu dapibus. Cras quis ipsum maximus mi dignissim lobortis quis id ligula. Donec auctor ante sed vehicula iaculis. Sed ac faucibus ligula. Quisque pellentesque malesuada tincidunt. Curabitur egestas consectetur ipsum, eget pretium ipsum elementum consequat. Quisque condimentum enim ante, vitae aliquet purus viverra in.\r\n\r\nVivamus sit amet sem a arcu consectetur efficitur. Pellentesque eu porttitor ipsum, quis dictum enim. Sed non purus convallis, venenatis augue tincidunt, lobortis tortor. Morbi vitae turpis sagittis, auctor ipsum laoreet, tristique justo. Sed consequat tincidunt nisi vitae pellentesque. Pellentesque id massa eget urna hendrerit posuere a posuere lacus. Nunc vitae aliquet sapien, eget eleifend erat. Morbi commodo tincidunt dolor. Mauris et turpis quis nisl blandit porta. Sed rutrum dui quis mattis feugiat. Quisque id massa ac lacus gravida accumsan non non dolor.\r\n\r\nNulla convallis leo metus, sed aliquam mauris luctus non. Suspendisse commodo ante eu faucibus placerat. Curabitur in dapibus enim, sit amet pretium sapien. Quisque imperdiet maximus augue, non ultricies ex interdum lacinia. Maecenas egestas justo nec est malesuada, sed blandit tortor aliquet. Nulla dignissim eros magna, a mattis velit accumsan vitae. Nullam ut accumsan est. Maecenas eu tellus vitae erat euismod fermentum sit amet non sapien. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.\r\n\r\nProin mattis sapien efficitur metus vestibulum, a volutpat ipsum lacinia. Nulla at laoreet ligula, nec pulvinar nunc. Nunc quis leo venenatis, vulputate erat eu, varius elit. Phasellus sed imperdiet augue. Morbi ullamcorper at ante sit amet faucibus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Aliquam accumsan feugiat neque in efficitur. Nunc convallis, ipsum a elementum imperdiet, dolor nunc hendrerit sem, eget eleifend mi arcu sed purus. Phasellus orci urna, egestas eget pulvinar nec, tempus semper lorem. Duis euismod, tellus mattis posuere hendrerit, metus enim sodales nisl, nec faucibus erat velit ut ex. Sed ornare arcu sed purus dictum, at hendrerit est congue. Nam semper, mauris sed vulputate aliquet, urna sapien pretium magna, vel ultricies erat purus et magna. In non nulla tincidunt, eleifend odio ut, sagittis lorem. Donec quis nisi a metus vestibulum laoreet. Nullam sed ipsum non arcu feugiat eleifend ac et sapien. Interdum et malesuada fames ac ante ipsum primis in faucibus. ";

    [Benchmark]
    public void GetCapitalizeSpan()
    {
        CapitalizeSpan(stringTest);
    }

    [Benchmark]
    public void GetCapitalizeStr()
    {
        CapitalizeStr(stringTest);
    }

    public static string CapitalizeStr(string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        return char.ToUpper(str[0]) + str[1..].ToLower();
    }

    public static string CapitalizeSpan(ReadOnlySpan<char> str)
    {
        if (str.IsEmpty) return string.Empty;
        var result = str.Length <= 256 ? stackalloc char[str.Length] : new char[str.Length];
        str.ToLower(result, CultureInfo.InvariantCulture);
        result[0] = char.ToUpperInvariant(str[0]);
        return result.ToString();
    }
}