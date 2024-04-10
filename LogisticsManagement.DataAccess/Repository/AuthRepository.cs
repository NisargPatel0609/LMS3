using LogisticsManagement.DataAccess.Models;
using LogisticsManagement.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;



namespace LogisticsManagement.DataAccess.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly LogisticsManagementContext _dbContext;
        public AuthRepository(LogisticsManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Add User to DB
        public async Task<int> AddUser(User user, UserDetail userDetail)
        {
            try
            {
                await _dbContext.Users.AddAsync(user);
                await _dbContext.UserDetails.AddAsync(userDetail);
                if (await _dbContext.SaveChangesAsync() > 0)
                    return user.Id;
            }

            catch (Exception)
            {
                Console.WriteLine("An Error occured while adding user");
                return -2;
            }

            return 0;
        }


        //Get user details by email id
        public async Task<User?> GetUserByEmailId(string emailid)
        {
            try
            {

                return await _dbContext.Users.Include(u => u.Role)
                                             .Include(u => u.UserDetails)
                                             .ThenInclude(u => u.Address)
                                             .ThenInclude(u => u.City)
                                             .ThenInclude(u => u.State)
                                             .ThenInclude(u => u.Country)
                                             .Where(u => u.Email == emailid && u.IsActive==true).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while fetching user by email id\n" + e.Message);
                return null;
            }
        }


    }
}
