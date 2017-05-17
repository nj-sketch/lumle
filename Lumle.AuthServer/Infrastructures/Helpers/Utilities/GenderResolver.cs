using static Lumle.AuthServer.Infrastructures.Enums.AuthEnums;

namespace Lumle.AuthServer.Infrastructures.Helpers.Utilities
{
    public static class GenderResolver
    {

        public static int GetNumericGenderValue(string gender)
        {
            if (string.IsNullOrEmpty(gender))
            {
                return 0;
            }

            switch (gender.ToLower())
            {

                case "male":
                    return (int)Gender.Male;
                case "female":
                    return (int)Gender.Female;
                case "other":
                    return (int)Gender.Other;
                default:
                    return (int)Gender.Unknown;
            }
        }

    }
}
