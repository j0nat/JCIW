using System;

namespace JCIW.Data.Interfaces
{
    public interface IJCIWGameComponent
    {
        void Initialize(object graphicsDevice);

        void Draw();

        void Update(TimeSpan elapsedGameTime);
    }
}
