﻿namespace BMCLV2.Mirrors.BMCLAPI
{
    public class Version : Interface.Version
    {
        public override string Name { get; } = "BMCLAPI";

        public Version()
        {
            Url = "http://bmclapi2.bangbang93.com/mc/game/version_manifest.json";
        }
    }
}