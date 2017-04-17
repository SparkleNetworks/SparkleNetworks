using System;
using Sparkle.Data.Networks;

namespace Sparkle.Data.Stub
{
    public partial class StubRepositoryFactory :IDisposable
    {
        private bool disposed;

        public StubRepositoryFactory()
        {
        }

        #region IDisposable members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
                this.disposed = true;
            }
        }

        #endregion

        public IAchievementsRepository Achievements
        {
            get { throw new NotImplementedException(); }
        }

        public IAchievementsUsersRepository AchievementsUsers
        {
            get { throw new NotImplementedException(); }
        }

        public ILunchRepository Lunch
        {
            get { throw new NotImplementedException(); }
        }


        public IExchangeMaterialsRepository ExchangeMaterials {
            get { throw new NotImplementedException(); }
        }

        public IExchangeSkillsRepository ExchangeSkills {
            get { throw new NotImplementedException(); }
        }

        public IExchangeSurfacesRepository ExchangeSurfaces {
            get { throw new NotImplementedException(); }
        }

        public ISparkleServicesRepository SparkleServices {
            get { throw new NotImplementedException(); }
        }
    }
}
