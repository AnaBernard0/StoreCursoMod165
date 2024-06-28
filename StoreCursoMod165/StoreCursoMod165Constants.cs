namespace StoreCursoMod165
{
    //valores estaticos existem semore e sao logo inicializados
    public static class StoreCursoMod165Constants //static significa vai construir uma instancia do cursoMod165 que ja esta inicializado
    {
        public readonly struct USERS
        {
            public readonly struct ADMINISTRADOR
            {
                public static readonly string USERNAME = "administrador";
                public static readonly string PASSWORD = "xpto1234";
            }

            public readonly struct VENDEDOR
            {
                public static readonly string USERNAME = "vendedor";
                public static readonly string PASSWORD = "12345678";
            }

            public readonly struct LOGISTICA
            {
                public static readonly string USERNAME = "logistica";
                public static readonly string PASSWORD = "20034065";
            }

        }

        public readonly struct ROLES
        {
           
            public static readonly string ADMINISTRADOR = "ADMINISTRADOR";
            public static readonly string VENDEDOR = "VENDEDOR";
            public static readonly string LOGISTICA = "LOGISTICA";

        }

        public readonly struct POLICIES
        {

            public readonly struct APP_POLICY_VENDEDORES
            {
                public const string NAME = "APP_POLICY_VENDEDORES";
                public static readonly string[] APP_POLICY_ROLES =
                {

                    ROLES.ADMINISTRADOR,
                    ROLES.VENDEDOR,
                };
            }

            public readonly struct APP_POLICY_ADMIN
            {
                public const string NAME = "APP_POLICY_ADMIN";
                public static readonly string[] APP_POLICY_ROLES =
                {
                    ROLES.ADMINISTRADOR,
                };
            }
        }
    }
}

