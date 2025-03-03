using System.Collections;
using System.Collections.Generic;

public class BaseRandomProbabiltty
{
    private List<long> randomValueList;
    private const int BitWindowSize = 16; 
    private const int UsedCount = 3;
    private int offset;
    private int count;
    private int index;

    public BaseRandomProbabiltty(List<long> seeds)
    {
        randomValueList = seeds;
        offset = 0;
        count = 0;
        index = 0;
    }

    public long GetRandomValue()
    {
        if (randomValueList == null || randomValueList.Count == 0) return -1;

        int currentOffset = offset + (BitWindowSize * count);

        if(currentOffset + BitWindowSize > 64)
        {
            index++;
            offset = 0;
            count = 0;

            if (index >= randomValueList.Count)
            {
                index = 0;
            }
        }

        long seed = randomValueList[index];
        long mask = ((1L << BitWindowSize) - 1);
        long ret = (seed >> currentOffset) & mask;

        count++;
        if (count >= UsedCount)
        {
            offset++;
            count = 0;
        }

        return ret;
    }

    public void Init(List<long> seeds)
    {
        randomValueList = seeds;
        offset = 0;
        count = 0;
        index = 0;
    }
}
