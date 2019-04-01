using ElfKingdom;

namespace MyBot
{
    abstract class StrategicCalculations : Math
    {
        /// <summary>
        /// how much mana has been wated from the start
        /// </summary>
        public int ManaWasted { get; set; }
        /// <summary>
        /// allocated mana for portals
        /// </summary>
        protected static int AllocatedMana { get; set; } = 0;
        /// <summary>
        /// the angle between castles
        /// </summary>
        protected double BaseDegree { get; private set; }
        /// <summary>
        /// the first defend circle, only when we don't have any target
        /// </summary>
        protected int DefendRadius { get; private set; }
        /// <summary>
        /// how much portals do we want to have, when amount is acommplished priorty lower
        /// </summary>
        protected int DesiredPortalAmount { get; private set; }
        /// <summary>
        /// the amount of mana we want per turn, when acopmmplished priorty lower
        /// </summary>
        protected int DesiredManaPerTurn { get; private set; }
        /// <summary>
        /// the max amount of mana we want per turn, when acopmmplished priorty zero
        /// </summary>
        protected int IdealManaPerTurn { get; private set; }
        /// <summary>
        /// the max summon rate of icetroll
        /// </summary>
        protected int IceTrollSummonRate { get; private set; }
        /// <summary>
        /// turn 600, the big attack
        /// </summary>
        protected int TheLongestDay { get; private set; }
        /// <summary>
        /// the min radius for build portals
        /// </summary>
        protected int MinPortalBuildRadius { get; private set; }
        /// <summary>
        /// the min radius for build mana fountain
        /// </summary>
        protected int MinFountainBuildRadius { get; private set; }
        /// <summary>
        /// the max radius for build portals
        /// </summary>
        protected int MaxBuildRadius { get; private set; }
        /// <summary>
        /// the min amount of mana, when acommplished build a portal
        /// </summary>
        protected int MinManaForPortal { get; private set; }
        /// <summary>
        /// the amount of the expect mana on turn 600, in relation to the currnet mana per turn
        /// </summary>
        protected int MaxPotentialMana { get; private set; }
        /// <summary>
        /// how much health points we expect in panic mode, when is coming to panic point start big attack 
        /// </summary>
        protected int PanicTrigger { get; private set; }
        /// <summary>
        /// any portal in the range considered as deffend portal 
        /// need the change the name
        /// </summary>
        protected int BadMarginOfError { get; private set; }
        /// <summary>
        /// minimum range from castle when enemy portal considered as threat
        /// </summary>
        protected int EnemyAggressivePortalRangeFromCastle { get; private set; }
        /// <summary>
        /// minimum range from elf when enemy portal considered as threat
        /// </summary>
        protected int EnemyAggressivePortalRangeFromElf { get; private set; }
        /// <summary>
        /// minimum range from portal when enemy portal considered as threat
        /// </summary>
        protected int EnemyAggressivePortalRangeFromPortal { get; private set; }
        protected int EnemyVeryAggressivePortalRangeFromPortal { get; private set; }
        protected int EnemyVeryAggressivePortalRangeFromCastle { get; private set; }
        protected int EnemyVeryAggressivePortalRangeFromElf { get; private set; }
        protected int EnemyAggressiveElfRangeFromCastle { get; private set; }
        protected int EnemyAggressiveElfRangeFromElf { get; private set; }
        protected int EnemyAggressiveElfRangeFromPortal { get; private set; }
        protected int EnemyAggressiveTornadoRangeFromPortal { get; private set; }
        protected int EnemyVeryAggressiveTornadoRangeFromPortal { get; private set; }
        protected int EnemyAggressiveLavaGiantRangeFromCastle { get; private set; }
        protected int EnemyAggressiveLavaGiantRangeFromElf { get; private set; }

        public abstract void DoTurn(Game game);
        public void CalculateConsts(Game game)
        {
            CalculateBaseDegree(game);
        }
        public void CalculateAll(Game game)
        {
            if (game.Turn <= 1)
            {
                CalculateConsts(game);
            }
            CalculateTheLongestDay(game);
            CalculateMinPortalBuildRadius(game);
            CalculateMinFountainBuildRadius(game);
            CalculateMaxBuildRadius(game);
            CalculateDefendRadius(game);
            CalculateDesiredPortalAmount(game);
            CalculateDesiredManaPerTurn(game);
            CalculateIdealManaPerTurn(game);
            CalculateIceTrollSummonRate(game);
            CalculateMinManaForPortal(game);
            CalculateMaxPotentialMana(game);
            CalculatePanicTrigger(game);
            CalculateBadMarginOfError(game);
            CalculateEnemyAggressivePortalRangeFromCastle(game);
            CalculateEnemyAggressivePortalRangeFromElf(game);
            CalculateEnemyAggressivePortalRangeFromPortal(game);
            CalculateEnemyVeryAggressivePortalRangeFromCastle(game);
            CalculateEnemyVeryAggressivePortalRangeFromElf(game);
            CalculateEnemyVeryAggressivePortalRangeFromPortal(game);
            CalculateEnemyAggressiveElfRangeFromCastle(game);
            CalculateEnemyAggressiveElfRangeFromElf(game);
            CalculateEnemyAggressiveElfRangeFromPortal(game);
            CalculateEnemyAggressiveTornadoRangeFromPortal(game);
            CalculateEnemyVeryAggressiveTornadoRangeFromPortal(game);
            CalculateEnemyAggressiveLavaGiantRangeFromCastle(game);
            CalculateEnemyAggressiveLavaGiantRangeFromElf(game);
        }
        protected Location Cis(double radius, double degree, Location baseLocation = null)
        {
            if (baseLocation == null)
            {
                return new Location((int)(radius * Sin(degree)), (int)(radius * Cos(degree)));
            }
            else
            {
                return new Location(baseLocation.Row + (int)(radius * Sin(degree)), baseLocation.Col + (int)(radius * Cos(degree)));
            }
        }
        protected double DegreeBetween(Location a, Location b)
        {
            double a1 = Sqrt(Pow(a.Col - b.Col, 2) + Pow(a.Row - b.Row, 2));
            double b1 = Abs(a.Col - b.Col);
            return (a.Col > b.Col ? 3 * PI / 2 : PI / 2) + Asin(b1 / a1);
        }
        private void CalculateBaseDegree(Game game)
        {
            Location baseLocation = game.GetMyCastle().Location;
            double a = Sqrt(Pow(baseLocation.Col - game.GetEnemyCastle().Location.Col, 2) + Pow(baseLocation.Row - game.GetEnemyCastle().Location.Row, 2));
            double b = Abs(baseLocation.Col - game.GetEnemyCastle().Location.Col);
            BaseDegree = Asin(b / a);
            BaseDegree = PI / 40;
            int min = int.MaxValue;
            for (int i = 0; i < 80; i++)
            {
                if (game.GetEnemyCastle().Distance(Cis(100, PI / 40 + PI * i / 40, game.GetMyCastle().Location)) < min)
                {
                    min = game.GetEnemyCastle().Distance(Cis(100, PI / 40 + PI * i / 40, game.GetMyCastle().Location));
                    BaseDegree = PI / 40 + PI * i / 40;
                }
            }
        }
        private void CalculateDefendRadius(Game game)
        {
            //DefendRadius = ((game.ElfMaxHealth / game.ElfAttackMultiplier) * game.ElfMaxSpeed / 2) + game.PortalSize + game.CastleSize;
            int maxDistance = -1;
            GameObject myCastle = game.GetMyCastle();
            foreach (Portal portal in game.GetMyPortals())
            {
                if (portal.Distance(game.GetMyCastle()) > maxDistance)
                {
                    maxDistance = portal.Distance(game.GetMyCastle());
                }
            }
            if (maxDistance != -1)
            {
                if (maxDistance < MaxBuildRadius)
                {
                    DefendRadius = maxDistance;
                }
                else
                {
                    DefendRadius = MaxBuildRadius;
                }
            }
            else
            {
                DefendRadius = ((game.ElfMaxHealth / game.ElfAttackMultiplier) * game.ElfMaxSpeed / 2) + game.PortalSize + game.CastleSize;
            }
        }
        private void CalculateIceTrollSummonRate(Game game)
        {
            try
            {
                IceTrollSummonRate = (int)Floor((decimal)((game.IceTrollCost / game.GetMyself().ManaPerTurn) * 1.6));
            }
            catch
            {
                IceTrollSummonRate = 8;//(int)System.Math.Ceiling((decimal)((game.IceTrollCost / game.DefaultManaPerTurn) * 1.6));
            }
            if (game.GetMyCastle().CurrentHealth <= PanicTrigger)
            {
                IceTrollSummonRate /= 2;
            }
        }
        private void CalculateTheLongestDay(Game game)
        {
            TheLongestDay = (int)(game.MaxTurns * 0.8);
        }
        private void CalculateMaxPotentialMana(Game game)
        {
            MaxPotentialMana = TheLongestDay * game.GetMyself().ManaPerTurn;
        }
        private void CalculateMinPortalBuildRadius(Game game)
        {
            MinPortalBuildRadius = (int)(game.PortalSize * 2.25 + game.CastleSize + game.ManaFountainSize * 2);
        }
        private void CalculateMinFountainBuildRadius(Game game)
        {
            MinFountainBuildRadius = (int)(game.ManaFountainSize * 2.24 + game.CastleSize);
        }
        private void CalculateMaxBuildRadius(Game game)
        {
            MaxBuildRadius = 5000 + (int)(game.PortalSize * 2.25 + game.CastleSize);
        }
        private void CalculateMinManaForPortal(Game game)
        {
            if (game.GetMyself().ManaPerTurn <= DesiredManaPerTurn)
            {
                MinManaForPortal = game.ManaFountainCost * ((game.ManaFountainManaPerTurn * (1 + (8 - game.DefaultManaPerTurn) / game.ManaFountainManaPerTurn) - (game.GetMyself().ManaPerTurn - game.DefaultManaPerTurn)) / game.ManaFountainManaPerTurn);
            }
            else
            {
                MinManaForPortal = game.PortalCost;
            }
        }
        private void CalculateDesiredPortalAmount(Game game)
        {
            DesiredPortalAmount = 4;
        }
        private void CalculateDesiredManaPerTurn(Game game)
        {
            if (game.GetAllEnemyElves().Length == 3)
            {
                DesiredManaPerTurn = 13;
            }
            else
            {
                DesiredManaPerTurn = 8;
            }
        }
        private void CalculateIdealManaPerTurn(Game game)
        {
            if (game.GetAllEnemyElves().Length == 3)
            {
                IdealManaPerTurn = 16;
            }
            else
            {
                IdealManaPerTurn = 11;
            }
        }
        private void CalculatePanicTrigger(Game game)
        {
            if (true)
            {
                PanicTrigger = 50;
            }
            else
            {
                PanicTrigger = 40;
            }
        }
        private void CalculateBadMarginOfError(Game game)
        {
            BadMarginOfError = 1500;
        }
        private void CalculateEnemyAggressivePortalRangeFromCastle(Game game)
        {
            EnemyAggressivePortalRangeFromCastle = 3500;
        }
        private void CalculateEnemyAggressivePortalRangeFromElf(Game game)
        {
            EnemyAggressivePortalRangeFromElf = 700;
        }
        private void CalculateEnemyAggressivePortalRangeFromPortal(Game game)
        {
            EnemyAggressivePortalRangeFromPortal = game.PortalSize * 3 + 100;
        }
        private void CalculateEnemyVeryAggressivePortalRangeFromPortal(Game game)
        {
            EnemyVeryAggressivePortalRangeFromPortal = game.PortalSize * 2 + 100;
        }
        private void CalculateEnemyVeryAggressivePortalRangeFromCastle(Game game)
        {
            EnemyVeryAggressivePortalRangeFromCastle = (int)(EnemyAggressivePortalRangeFromCastle / 1.5);
        }
        private void CalculateEnemyVeryAggressivePortalRangeFromElf(Game game)
        {
            EnemyVeryAggressivePortalRangeFromElf = (int)(EnemyAggressivePortalRangeFromElf);
        }
        private void CalculateEnemyAggressiveElfRangeFromCastle(Game game)
        {
            //1500
            EnemyAggressiveElfRangeFromCastle = game.CastleSize + game.ElfAttackRange + game.ElfMaxSpeed * IceTrollSummonRate;
            //1600
        }
        private void CalculateEnemyAggressiveElfRangeFromElf(Game game)
        {
            EnemyAggressiveElfRangeFromElf = 500;
        }
        private void CalculateEnemyAggressiveElfRangeFromPortal(Game game)
        {
            //750
            EnemyAggressiveElfRangeFromPortal = game.PortalSize + game.ElfAttackRange + game.ElfMaxSpeed * IceTrollSummonRate;
            //1400
        }
        private void CalculateEnemyAggressiveTornadoRangeFromPortal(Game game)
        {
            //750
            EnemyAggressiveTornadoRangeFromPortal = game.PortalSize + game.ElfAttackRange + game.ElfMaxSpeed * IceTrollSummonRate;
            //1400
        }
        private void CalculateEnemyVeryAggressiveTornadoRangeFromPortal(Game game)
        {
            //750
            EnemyVeryAggressiveTornadoRangeFromPortal = game.PortalSize + game.ElfAttackRange + game.ElfMaxSpeed * IceTrollSummonRate;
            //1400
        }
        private void CalculateEnemyAggressiveLavaGiantRangeFromCastle(Game game)
        {
            EnemyAggressiveLavaGiantRangeFromCastle = 1500;
        }
        private void CalculateEnemyAggressiveLavaGiantRangeFromElf(Game game)
        {
            EnemyAggressiveLavaGiantRangeFromElf = 500;
        }
    }
}
