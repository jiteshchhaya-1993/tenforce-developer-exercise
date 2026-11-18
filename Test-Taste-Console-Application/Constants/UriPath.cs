namespace Test_Taste_Console_Application.Constants
{
    public static class UriPath
    {
        public const string BaseUri = "https://api.le-systeme-solaire.net";
        private const string BodiesUri = "/rest/bodies";

        public const string GetAllPlanetsWithMoonsQueryParameters =
            BodiesUri + "?data=id,semiMajorAxis,moons,moon,rel&filter[]=isPlanet,neq,false";

        // Fixed duplicate massValue and added avgTemp and gravity for complete moon information
        public const string GetAllMoonsWithMassQueryParameters = BodiesUri +
            "?data=id,mass,massValue,massExponent,avgTemp,gravity&filter[]=aroundPlanet,gt,null";

        // Bulk endpoint to fetch all moons in one query to prevent N+1 API calls
        public const string GetAllMoonsDataQueryParameters = BodiesUri +
            "?data=id,name,avgTemp,gravity,mass,massValue,massExponent,aroundPlanet&filter[]=aroundPlanet,gt,null";

        public const string GetMoonByIdQueryParameters = BodiesUri + "/";
    }
}
