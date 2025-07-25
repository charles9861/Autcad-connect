using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using MySql.Data.MySqlClient;

[assembly: CommandClass(typeof(SyncToDBCommand))]

public class SyncToDBCommand
{
    [CommandMethod("SYNC_TO_DB")]
    public void SyncEntitiesToDB()
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Editor ed = doc.Editor;
        Database db = doc.Database;

        string connStr = "server=localhost;uid=root;pwd=yourpass;database=AutoCADObjects;";
        using MySqlConnection conn = new MySqlConnection(connStr);
        conn.Open();

        using Transaction tr = db.TransactionManager.StartTransaction();
        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

        foreach (ObjectId id in ms)
        {
            Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
            if (ent == null || ent.XData == null) continue;

            string objectId = id.Handle.ToString();
            string attribute1 = GetXDataValue(ent.XData, 1); // 0 = RegAppName, 1 = first attribute

            if (!string.IsNullOrEmpty(attribute1))
            {
                string query = "REPLACE INTO DrawingObjects (ObjectID, CustomAttribute1) VALUES (@id, @attr)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", objectId);
                cmd.Parameters.AddWithValue("@attr", attribute1);
                cmd.ExecuteNonQuery();
            }
        }

        tr.Commit();
        ed.WriteMessage("\nSync completed.");
    }

    private string GetXDataValue(ResultBuffer rb, int index)
    {
        TypedValue[] data = rb.AsArray();
        return (data.Length > index) ? data[index].Value.ToString() : string.Empty;
    }
}
