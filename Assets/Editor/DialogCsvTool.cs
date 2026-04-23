using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DialogCsvTool : EditorWindow
{
    private string csvPath = "Assets/Dialog/dialog.csv";
    private string outputFolder = "Assets/Dialog";
    private string exportFolder = "Assets/DialogExport";

    [MenuItem("Tools/Dialog CSV Tool")]
    public static void ShowWindow()
    {
        GetWindow<DialogCsvTool>("Dialog CSV Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Dialog CSV Import / Export", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        csvPath = EditorGUILayout.TextField("CSV Path", csvPath);
        outputFolder = EditorGUILayout.TextField("Import Output Folder", outputFolder);
        exportFolder = EditorGUILayout.TextField("Export Folder", exportFolder);

        EditorGUILayout.Space();

        if (GUILayout.Button("Import CSV -> DialogData_SO"))
        {
            ImportCsvToDialogAssets(csvPath, outputFolder);
        }

        if (GUILayout.Button("Export DialogData_SO -> CSV"))
        {
            ExportDialogAssetsToCsv(outputFolder, exportFolder);
        }
    }

    private class CsvRow
    {
        public string dialogName;
        public int lineIndex;
        public string id;
        public string text;
        public string imagePath;
    }

    public static void ImportCsvToDialogAssets(string csvFilePath, string assetOutputFolder)
    {
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError($"[DialogCsvTool] CSV file not found: {csvFilePath}");
            return;
        }

        EnsureFolderExists(assetOutputFolder);

        string csvContent = File.ReadAllText(csvFilePath, Encoding.UTF8);
        string[] lines = csvContent.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        if (lines.Length <= 1)
        {
            Debug.LogWarning("[DialogCsvTool] CSV is empty or only contains header.");
            return;
        }

        List<CsvRow> rows = new List<CsvRow>();

        for (int i = 1; i < lines.Length; i++)
        {
            string rawLine = lines[i];
            if (string.IsNullOrWhiteSpace(rawLine))
                continue;

            string[] cols = ParseCsvLine(rawLine);

            if (cols.Length < 5)
            {
                Debug.LogWarning($"[DialogCsvTool] Line {i + 1} skipped. Need 5 columns: {rawLine}");
                continue;
            }

            if (!int.TryParse(cols[1], out int lineIndex))
            {
                Debug.LogWarning($"[DialogCsvTool] Invalid LineIndex at line {i + 1}: {rawLine}");
                continue;
            }

            CsvRow row = new CsvRow
            {
                dialogName = cols[0].Trim(),
                lineIndex = lineIndex,
                id = cols[2],
                text = cols[3],
                imagePath = cols[4]
            };

            rows.Add(row);
        }

        var grouped = rows.GroupBy(r => r.dialogName);

        int createdCount = 0;
        int updatedCount = 0;

        foreach (var group in grouped)
        {
            string dialogName = group.Key;
            if (string.IsNullOrWhiteSpace(dialogName))
            {
                Debug.LogWarning("[DialogCsvTool] Found empty DialogName, skipped.");
                continue;
            }

            string assetPath = $"{assetOutputFolder}/{dialogName}.asset";
            DialogData_SO dialogAsset = AssetDatabase.LoadAssetAtPath<DialogData_SO>(assetPath);

            bool isNew = false;
            if (dialogAsset == null)
            {
                dialogAsset = ScriptableObject.CreateInstance<DialogData_SO>();
                AssetDatabase.CreateAsset(dialogAsset, assetPath);
                isNew = true;
            }

            dialogAsset.dialogPieces.Clear();

            foreach (CsvRow row in group.OrderBy(r => r.lineIndex))
            {
                DialogPiece piece = new DialogPiece
                {
                    ID = row.id,
                    text = row.text,
                    image = LoadSprite(row.imagePath)
                };

                dialogAsset.dialogPieces.Add(piece);
            }

            EditorUtility.SetDirty(dialogAsset);

            if (isNew) createdCount++;
            else updatedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[DialogCsvTool] Import complete. Created: {createdCount}, Updated: {updatedCount}");
    }

    public static void ExportDialogAssetsToCsv(string dialogFolder, string csvOutputFolder)
    {
        EnsureFolderExists(csvOutputFolder);

        string[] guids = AssetDatabase.FindAssets("t:DialogData_SO", new[] { dialogFolder });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"[DialogCsvTool] No DialogData_SO found in folder: {dialogFolder}");
            return;
        }

        string csvFilePath = Path.Combine(csvOutputFolder, "dialog_export.csv");

        List<string> lines = new List<string>
        {
            "DialogName,LineIndex,ID,Text,ImagePath"
        };

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            DialogData_SO dialogAsset = AssetDatabase.LoadAssetAtPath<DialogData_SO>(assetPath);
            if (dialogAsset == null)
                continue;

            string dialogName = dialogAsset.name;

            for (int i = 0; i < dialogAsset.dialogPieces.Count; i++)
            {
                DialogPiece piece = dialogAsset.dialogPieces[i];

                string id = EscapeCsv(piece != null ? piece.ID : "");
                string text = EscapeCsv(piece != null ? piece.text : "");
                string imagePath = "";

                if (piece != null && piece.image != null)
                {
                    imagePath = EscapeCsv(AssetDatabase.GetAssetPath(piece.image));
                }

                lines.Add($"{EscapeCsv(dialogName)},{i},{id},{text},{imagePath}");
            }
        }

        File.WriteAllText(csvFilePath, string.Join("\n", lines), new UTF8Encoding(true));
        AssetDatabase.Refresh();

        Debug.Log($"[DialogCsvTool] Export complete: {csvFilePath}");
    }

    private static Sprite LoadSprite(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return null;

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath.Trim());
        if (sprite == null)
        {
            Debug.LogWarning($"[DialogCsvTool] Sprite not found: {imagePath}");
        }
        return sprite;
    }

    private static void EnsureFolderExists(string assetFolderPath)
    {
        if (AssetDatabase.IsValidFolder(assetFolderPath))
            return;

        string[] parts = assetFolderPath.Split('/');
        if (parts.Length < 2 || parts[0] != "Assets")
        {
            Debug.LogError($"[DialogCsvTool] Invalid folder path: {assetFolderPath}");
            return;
        }

        string current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }

    private static string[] ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);
        return result.ToArray();
    }

    private static string EscapeCsv(string value)
    {
        if (value == null)
            return "";

        bool needQuotes = value.Contains(",") || value.Contains("\"") || value.Contains("\n");
        value = value.Replace("\"", "\"\"");

        return needQuotes ? $"\"{value}\"" : value;
    }
}