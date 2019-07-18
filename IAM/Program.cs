using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Auth.AccessControlPolicy;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;

namespace IAM
{
    class Program
    {
        private const string SomeUser = "SomeUser";
        private const string AnotherUser = "AnotherUser";


        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome To IAM!\n\n");

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            using (IAmazonIdentityManagementService client =
                new AmazonIdentityManagementServiceClient(RegionEndpoint.USEast1))
            {
                //await CreateIaMUsers(client, token);
                //await AddUsersToAdminGroup(client, token);
                await AttachAdminPolicyToAdminGroup(client, token);

                Console.ReadKey();


            }
        }

        private static async Task AttachAdminPolicyToAdminGroup(IAmazonIdentityManagementService client, 
            CancellationToken token)
        {
            const string policyDocument =
                @"
                {
                    ""Version"": ""2012-10-17"",
                    ""Statement"": [
                        {
                            ""Effect"": ""Allow"",
                            ""Action"": ""*"",
                            ""Resource"": ""*""
                        }
                    ]
                }";

            CreatePolicyRequest request = new CreatePolicyRequest()
            {
                Description = "Policy for Administrators",
                PolicyDocument = policyDocument,
                PolicyName = "AllAccess"
            };

            CreatePolicyResponse response = await client.CreatePolicyAsync(request, token);
            //throw new NotImplementedException();
        }

        private static async Task CreateIaMUsers(IAmazonIdentityManagementService client,
            CancellationToken token)
        {
            CreateUserRequest request = new CreateUserRequest(SomeUser);
            CreateUserResponse response = await client.CreateUserAsync(request, token);

            Console.WriteLine($"The ARN of the new user is {response.User.Arn}");

            request = new CreateUserRequest(AnotherUser);
            response = await client.CreateUserAsync(request, token);

            Console.WriteLine($"The ARN of the new user is {response.User.Arn}");
        }


        private static async Task AddUsersToAdminGroup(IAmazonIdentityManagementService client, 
            CancellationToken token)
        {
            CreateGroupRequest groupRequest = new CreateGroupRequest("Admins");
            CreateGroupResponse response = await client.CreateGroupAsync(groupRequest, token);

            AddUserToGroupRequest request = new AddUserToGroupRequest("Admins", SomeUser);
            var foo = await client.AddUserToGroupAsync(request, token);

            request = new AddUserToGroupRequest("Admins", AnotherUser);
            foo = await client.AddUserToGroupAsync(request, token);
        }
    }
}
