using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;
namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "SizeSpringAppearance Appearance", menuName = "Text Animator/Animations/Appearances/SizeSpringAppearance")]
    [EffectInfo("sizespring", EffectCategory.Appearances)]
    public sealed class SizeSpringAppearance : AppearanceScriptableBase
    {
        float amplitude;
        public float baseAmplitude = 2;
        public AnimationCurve curve;
        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);
            amplitude = baseAmplitude * -1 + 1;
        }

        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
           var scale = (curve.Evaluate(character.passedTime / duration) +1) * amplitude;
            character.current.positions.LerpUnclamped(
                character.current.positions.GetMiddlePos(),scale +1
            );
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "a": amplitude = baseAmplitude * modifier.value; break;
                default: base.SetModifier(modifier); break;
            }
        }
    }
}