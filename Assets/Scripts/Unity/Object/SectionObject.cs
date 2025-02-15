using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{

    public class SectionObject : MonoBehaviour
    {
        Logic.Section _section;

        void Start()
        {
        }

        void Update()
        {
        }

        public void SetSection(Logic.Section section)
        {
            _section = section;
            if (_section != null)
            {
                var position = _section.GetSectionWorldPosition();
                transform.position = new Vector3(position.X, position.Y, 0);
                _section.ActiveSkill = SkillActive;
            }
        }

        public void SkillActive(Logic.Skill skill)
        {
            GameObject effectObject = Managers.Resource.Instantiate("Effect/Hit_04");
            var position = _section.GetSectionWorldPosition();
            effectObject.transform.position = new Vector3(position.X, position.Y, 0);
            var ps = effectObject.GetComponent<ParticleSystemRenderer>();
            ps.sortingLayerName = "Effect"; 
            ps.sortingOrder = 10;
        }
    }
}
