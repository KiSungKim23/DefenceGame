using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class SectionManager
    {
        private StageLogic _stageLogic;

        private Dictionary<(int, int), Section> _sectionDatas = new Dictionary<(int, int), Section>();

        public SectionManager(StageLogic stageLogic)
        {
            _stageLogic = stageLogic;
        }


        public Dictionary<(int, int), Section> SectionDatas
        {
            get
            {
                if (_sectionDatas.Count == 0)
                {
                    SetSectionData();
                }

                return _sectionDatas;
            }
        }

        public void Init()
        {
            SetSectionData();
        }

        public void SetSectionData()
        {
            if (_sectionDatas.Count == 0)
            {
                for (int i = 0; i < Define.SectionCount; i++)
                {
                    for (int j = 0; j < Define.SectionCount; j++)
                    {
                        if (i == 0 || i == Define.SectionCount - 1 || j == 0 || j == Define.SectionCount - 1)
                        {
                            _sectionDatas.Add((i, j), new Section(_stageLogic, (i, j)));
                        }
                    }
                }
            }
        }

        public void Update(long updateTick)
        {
            foreach (var section in _sectionDatas)
            {
                section.Value.Update(updateTick);
            }
        }

        public long CheckSkillActiveTick(long checkTick)
        {
            long ret = checkTick;

            foreach (var section in _sectionDatas)
            {
                long sectionCheckTick = section.Value.CheckActiveSkillTick();
                ret = ((sectionCheckTick == 0) || (ret < sectionCheckTick)) ? ret : sectionCheckTick;
            }

            return ret;
        }

        public Section GetSectionData((int,int) index)
        {
            if (_sectionDatas.TryGetValue(index, out var ret))
                return ret;

            return null;
        }
    }
}
