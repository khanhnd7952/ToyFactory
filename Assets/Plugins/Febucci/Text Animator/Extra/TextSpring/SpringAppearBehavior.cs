using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Springs Offset Appearance", menuName = "Text Animator/Animations/Appearances/SpringOffsetAppearBehavior")]
    [EffectInfo("offset", EffectCategory.Appearances)]
    public sealed class SpringAppearBehavior : AppearanceScriptableBase
    {
        public float baseAmount = 10;
        float amount;
        public Vector2 baseDirection = Vector2.one;
        public AnimationCurve curve;
        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);
            amount = baseAmount;
        }

        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
           var move = Mathf.Lerp(0, amount, curve.Evaluate(character.passedTime / duration));
            character.current.positions.MoveChar(baseDirection * move * character.uniformIntensity );
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "a": amount = baseAmount * modifier.value; break;
                default: base.SetModifier(modifier); break;
            }
        }
    }

}