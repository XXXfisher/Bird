using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DialogCsvTool : EditorWindow
{
    private string csvPath = "Assets/Dialog/dialog.csv";
    private string outputFolder = "Assets/Dialog";
    private string exportFolder = "Assets/Dialog";

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

    [Serializable]
    private class CsvRow
    {
        public string dialogName;
        public int lineIndex;
        public string speakerName;
        public string text;
        public string portraitSprite;
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
        csvContent = csvContent.TrimStart('\uFEFF');

        List<string[]> records = ParseCsvRecords(csvContent);

        if (records.Count <= 1)
        {
            Debug.LogWarning("[DialogCsvTool] CSV is empty or only contains header.");
            return;
        }

        List<CsvRow> rows = new List<CsvRow>();

        for (int i = 1; i < records.Count; i++) // Ìø¹ý±íÍ·
        {
            string[] cols = records[i];

            if (cols == null || cols.Length == 0)
                continue;

            if (IsRecordCompletelyEmpty(cols))
                continue;

            if (cols.Length < 5)
            {
                Debug.LogWarning($"[DialogCsvTool] Record {i + 1} skipped. Need 5 columns.");
                continue;
            }

            string dialogName = cols[0].Trim();
            string lineIndexRaw = cols[1].Trim();
            string speakerName = cols[2];
            string text = cols[3];
            string portraitSprite = cols[4];

            if (string.IsNullOrWhiteSpace(dialogName))
            {
                Debug.LogWarning($"[DialogCsvTool] Record {i + 1} skipped. DialogName is empty.");
                continue;
            }

            if (!int.TryParse(lineIndexRaw, out int lineIndex))
            {
                Debug.LogWarning($"[DialogCsvTool] Record {i + 1} skipped. Invalid LineIndex: {lineIndexRaw}");
                continue;
            }

            CsvRow row = new CsvRow
            {
                dialogName = dialogName,
                lineIndex = lineIndex,
                speakerName = speakerName,
                text = text,
                portraitSprite = portraitSprite
            };

            rows.Add(row);
        }

        if (rows.Count == 0)
        {
            Debug.LogWarning("[DialogCsvTool] No valid dialog rows found in CSV.");
            return;
        }

        var grouped = rows.GroupBy(r => r.dialogName);

        int createdCount = 0;
        int updatedCount = 0;

        foreach (var group in grouped)
        {
            string dialogName = group.Key;
            string assetPath = $"{assetOutputFolder}/{dialogName}.asset";

            DialogData_SO dialogAsset = AssetDatabase.LoadAssetAtPath<DialogData_SO>(assetPath);

            bool isNew = false;
            if (dialogAsset == null)
            {
                dialogAsset = ScriptableObject.CreateInstance<DialogData_SO>();
                AssetDatabase.CreateAsset(dialogAsset, assetPath);
                isNew = true;
            }

            if (dialogAsset.dialogPieces == null)
            {
                dialogAsset.dialogPieces = new List<DialogPiece>();
            }
            else
            {
                dialogAsset.dialogPieces.Clear();
            }

            foreach (CsvRow row in group.OrderBy(r => r.lineIndex))
            {
                DialogPiece piece = new DialogPiece
                {
                    ID = row.speakerName,
                    text = row.text,
                    image = LoadSprite(row.portraitSprite)
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
        if (guids == null || guids.Length == 0)
        {
            Debug.LogWarning($"[DialogCsvTool] No DialogData_SO found in folder: {dialogFolder}");
            return;
        }

        string csvFilePath = Path.Combine(csvOutputFolder, "dialog_export.csv");

        List<string> lines = new List<string>
        {
            "DialogName,LineIndex,SpeakerName,Text,PortraitSprite"
        };

        List<DialogData_SO> allAssets = new List<DialogData_SO>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            DialogData_SO dialogAsset = AssetDatabase.LoadAssetAtPath<DialogData_SO>(assetPath);
            if (dialogAsset != null)
            {
                allAssets.Add(dialogAsset);
            }
        }

        foreach (DialogData_SO dialogAsset in allAssets.OrderBy(a => a.name))
        {
            string dialogName = dialogAsset.name;

            if (dialogAsset.dialogPieces == null || dialogAsset.dialogPieces.Count == 0)
            {
                lines.Add($"{EscapeCsv(dialogName)},0,,,");
                continue;
            }

            for (int i = 0; i < dialogAsset.dialogPieces.Count; i++)
            {
                DialogPiece piece = dialogAsset.dialogPieces[i];

                string speakerName = EscapeCsv(piece != null ? piece.ID : "");
                string text = EscapeCsv(piece != null ? piece.text : "");
                string portraitSprite = "";

                if (piece != null && piece.image != null)
                {
                    portraitSprite = EscapeCsv(piece.image.name);
                }

                lines.Add($"{EscapeCsv(dialogName)},{i},{speakerName},{text},{portraitSprite}");
            }
        }

        string content = string.Join("\n", lines);
        File.WriteAllText(csvFilePath, content, new UTF8Encoding(true));

        AssetDatabase.Refresh();
        Debug.Log($"[DialogCsvTool] Export complete: {csvFilePath}");
    }

    private static Sprite LoadSprite(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return null;

        string trimmedPath = imagePath.Trim();
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(trimmedPath);

        if (sprite == null)
        {
            Debug.LogWarning($"[DialogCsvTool] Sprite not found: {trimmedPath}");
        }

        return sprite;
    }

    private static void EnsureFolderExists(string assetFolderPath)
    {
        if (AssetDatabase.IsValidFolder(assetFolderPath))
            return;

        string normalizedPath = assetFolderPath.Replace("\\", "/");

        string[] parts = normalizedPath.Split('/');
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

    private static List<string[]> ParseCsvRecords(string content)
    {
        List<string[]> records = new List<string[]>();
        List<string> currentRow = new List<string>();

        bool inQuotes = false;
        StringBuilder currentField = new StringBuilder();

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < content.Length && content[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                currentRow.Add(currentField.ToString());
                currentField.Clear();
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                if (c == '\r' && i + 1 < content.Length && content[i + 1] == '\n')
                {
                    i++;
                }

                currentRow.Add(currentField.ToString());
                currentField.Clear();

                if (!IsRowCompletelyEmpty(currentRow))
                {
                    records.Add(currentRow.ToArray());
                }

                currentRow = new List<string>();
            }
            else
            {
                currentField.Append(c);
            }
        }

        currentRow.Add(currentField.ToString());
        if (!IsRowCompletelyEmpty(currentRow))
        {
            records.Add(currentRow.ToArray());
        }

        return records;
    }

    private static bool IsRowCompletelyEmpty(List<string> row)
    {
        if (row == null || row.Count == 0)
            return true;

        for (int i = 0; i < row.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(row[i]))
                return false;
        }

        return true;
    }

    private static bool IsRecordCompletelyEmpty(string[] record)
    {
        if (record == null || record.Length == 0)
            return true;

        for (int i = 0; i < record.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(record[i]))
                return false;
        }

        return true;
    }

    private static string EscapeCsv(string value)
    {
        if (value == null)
            return "";

        bool needQuotes = value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r");
        value = value.Replace("\"", "\"\"");

        return needQuotes ? $"\"{value}\"" : value;
    }
}