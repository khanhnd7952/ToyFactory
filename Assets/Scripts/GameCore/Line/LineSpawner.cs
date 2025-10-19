using System.Collections.Generic;
using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    public static LineSpawner Instance { get; private set; }
    public EColorType[] SpawnData;

    public Line line1;
    public Line line2;
    public Line line3;
    

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnLine(EColorType[] Data, int[] Distributions)
    {
        SpawnData = Data;

        // Chia đều Data vào 3 line
        DistributeDataToLines();
    }
    
    private void DistributeDataToLines()
    {
        if (SpawnData == null || SpawnData.Length == 0) return;
        
        // Tạo array của Line references
        Line[] lines = { line1, line2, line3 };
        
        // Tạo list data cho từng line
        var line1Data = new List<EColorType>();
        var line2Data = new List<EColorType>();
        var line3Data = new List<EColorType>();
        
        // Chia đều theo kiểu round-robin: phần tử 0->line1, 1->line2, 2->line3, 3->line1, ...
        for (int i = 0; i < SpawnData.Length; i++)
        {
            int lineIndex = i % 9; // 0, 1, 2, 0, 1, 2, ...
            
            switch (lineIndex)
            {
                case 0:
                case 1:
                case 2:
                    line1Data.Add(SpawnData[i]);
                    break;
                case 3:
                case 4:
                case 5:  
                    line2Data.Add(SpawnData[i]);
                    break;
                case 6:
                case 7:
                case 8:  
                    line3Data.Add(SpawnData[i]);
                    break;
            }
        }
        
        // Gọi Line.Spawn cho từng line
        if (line1 != null && line1Data.Count > 0)
            line1.Spawn(line1Data.ToArray());
            
        if (line2 != null && line2Data.Count > 0)
            line2.Spawn(line2Data.ToArray());
            
        if (line3 != null && line3Data.Count > 0)
            line3.Spawn(line3Data.ToArray());
    }
}