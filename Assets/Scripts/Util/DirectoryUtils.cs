using System.IO;

/// <summary>
/// Directory クラスに関する汎用関数を管理するクラス
/// </summary>
public static class DirectoryUtils
{
    /// <summary>
    /// 指定したパスにディレクトリが存在しない場合
    /// すべてのディレクトリとサブディレクトリを作成します
    /// </summary>
    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }
}