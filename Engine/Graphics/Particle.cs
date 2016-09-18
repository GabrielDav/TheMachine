using System.Collections.Generic;
using Engine.Core;

namespace Engine.Graphics
{
    public class Particle
    {
        public List<IEffect> Effects;

        public Image Image;

        public bool Active;

        protected Timer _timer;

        public Particle(Image image, List<IEffect> effects)
        {
            Image = image;
            Effects = effects;
            _timer = new Timer(true);
        }

        public void Activate(int time)
        {
            _timer.Start(time, false);
            Active = true;
            Image.IsHidden = false;
        }

        public virtual void Update()
        {
            if (!Active)
            {
                return;
            }

            foreach (var effect in Effects)
            {
                effect.Update();
            }

            _timer.Update();
            if (_timer.Finished)
            {
                Active = false;
                Image.IsHidden = true;
            }
        }
    }
}
