using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using TestBucket.AdbProxy.Models;

namespace TestBucket.AdbProxy.Android;
public sealed class AndroidVersionProvider
{
    // language=json
    private static readonly string s_json = """
        [
            {
                "code_name": "Baklava",
                "version_name": "Android 16",
                "api_level": 36
            },
            {
                "code_name": "Vanilla Ice Cream",
                "version_name": "Android 15",
                "api_level": 35
            },
                    {
                "code_name": "Upside Down Cake",
                "version_name": "Android 14",
                "api_level": 34
            },
            {
                "code_name": "Tiramisu",
                "version_name": "Android 13",
                "api_level": 33
            },
                    {
                "code_name": "Snow Cone",
                "version_name": "Android 12",
                "api_level": 32
            },
            {
                "code_name": "Snow Cone",
                "version_name": "Android 11",
                "api_level": 31
            },
            {
                "code_name": "Red Velvet Cake",
                "version_name": "Android 11",
                "api_level": 30
            },
            {
                "code_name": "Quince Tart",
                "version_name": "Android 10",
                "api_level": 29
            },
            {
                "code_name": "Pie",
                "version_name": "Android 9",
                "api_level": 28
            },
            {
                "code_name": "Oreo",
                "version_name": "Android 8.1",
                "api_level": 27
            },
            {
                "code_name": "Oreo",
                "version_name": "Android 8.0",
                "api_level": 26
            },
            {
                "code_name": "Nougat",
                "version_name": "Android 7.1",
                "api_level": 25
            },
            {
                "code_name": "Nougat",
                "version_name": "Android 7.0",
                "api_level": 24
            },
            {
                "code_name": "Marshmallow",
                "version_name": "Android 6.0",
                "api_level": 23
            },
            {
                "code_name": "Lollipop",
                "version_name": "Android 5.1",
                "api_level": 22
            },
            {
                "code_name": "Lollipop",
                "version_name": "Android 5.0",
                "api_level": 21
            },
            {
                "code_name": "KitKat",
                "version_name": "Android 4.4",
                "api_level": 19
            }
        ]
        """;
    private static readonly List<AndroidVersion> s_versions;

    static AndroidVersionProvider()
    {
        s_versions = JsonSerializer.Deserialize<List<AndroidVersion>>(s_json) ?? new();
    }

    public static AndroidVersion FromApiLevel(int apiLevel)
    {
        var version = s_versions.Where(x=>x.ApiLevel == apiLevel).FirstOrDefault();
        return version ?? new AndroidVersion() {  ApiLevel = apiLevel };
    }
}
