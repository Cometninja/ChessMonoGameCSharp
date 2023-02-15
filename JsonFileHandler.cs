using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Diagnostics;

namespace Chess1
{
    internal class JsonFileHandler
    {
        public JsonFileHandler()
        {

        }

        public void SaveFile(SpriteChessBoard item)
        {
            string file = JsonSerializer.Serialize(item);

            File.WriteAllText("JsonTest.json", file);
        }
        public void Saveboard(List<SpriteChessBoard> items)
        {
            string jsonBoard = "";
            foreach (object item in items)
            {
                jsonBoard+= JsonSerializer.Serialize(item);
            }
            File.WriteAllText("JsonTest.json", jsonBoard);
        }

        public void SaveChessPiece(List<SpriteChessPiece> pieces) 
        {
            string content = "";
            foreach(SpriteChessPiece piece in pieces)
            {
                JsonChessPiece jsonChessPiece = new JsonChessPiece();
                jsonChessPiece.name = piece._image.ToString();
                jsonChessPiece.pointX =  piece._piecePoint.X;
                jsonChessPiece.pointY =  piece._piecePoint.Y;
                content += JsonSerializer.Serialize(jsonChessPiece);
            }
            File.WriteAllText("JsonChessPieces.json",content);


        }

        

    }
}
