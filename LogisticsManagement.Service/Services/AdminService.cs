using AutoMapper;
using LogisticsManagement.DataAccess.Models;
using LogisticsManagement.DataAccess.Repository.IRepository;
using LogisticsManagement.Service.DTOs;
using LogisticsManagement.Service.Enums;
using LogisticsManagement.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsManagement.Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public AdminService(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        // Get User By Id
        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            try
            {
                User? user = await _adminRepository.GetUserById(id);

                // check if user exists in database
                if (user is not null)
                {
                    return _mapper.Map<UserDTO>(user);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while getting user" + ex.Message);
                return null;
            }
        }

        // Get Users By Role
        public async Task<List<UserDTO>> GetUsersByRoleAsync(int roleId)
        {
            try
            {
                // check if roleId is defined in UserRoles enum
                if (Enum.IsDefined(typeof(UserRoles), roleId))
                {
                    List<User> users = await _adminRepository.GetUsersByRoleId(roleId);
                    if (users is not null || users?.Count > 0)
                    {
                        return _mapper.Map<List<UserDTO>>(users);
                    }
                }
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while getting users" + ex.Message);
                return null;
            }

        }

        // Get Users by Role whose Approval is Pending 
        public async Task<List<UserDTO>> GetPendingUsersByRoleAsync(int roleId)
        {
            try
            {
                // check if roleId is either manager or driver
                if (roleId == (int)UserRoles.Manager || roleId == (int)UserRoles.Driver)
                {
                    List<User> users = await _adminRepository.GetPendingUsersByRoleId(roleId);
                    if (users is not null || users?.Count > 0)
                    {
                        return _mapper.Map<List<UserDTO>>(users);
                    }
                }
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while getting users" + ex.Message);
                return null;
            }

        }

        // Approve or Reject manager's or driver's Sign Up Request
        public async Task<int> UpdateUserSignUpRequestAsync(int userId, int updatedStatus)
        {
            try
            {
                // check if userId is valid and updatedStatus is either -1 or 1
                if (userId <= 0 || !(updatedStatus is (int)SignUpStatus.Rejected || updatedStatus is (int)SignUpStatus.Approved))
                {
                    return -1;
                }
                User? user= await _adminRepository.GetUserById(userId);
                if (user?.RoleId != (int)UserRoles.Manager && user.RoleId != (int)UserRoles.Driver)
                    return 0;

                    return await _adminRepository.UpdateSignUpRequest(userId, updatedStatus);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while updating user" + ex.Message);
                return -1;
            }
        }

        // Delete User
        public async Task<int> DeleteUserAsync(int userId)
        {
            try
            {
                // check if userId is valid
                if (userId <= 0)
                {
                    return -1;
                }
                return await _adminRepository.DeleteUserById(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while deleting user" + ex.Message);
                return -1;
            }
        }

        // Assign Manager to Warehouse
        public async Task<int> AssignManagerToWarehouseAsync(int managerId, int warehouseId)
        {
            try
            {
                User? user = await _adminRepository.GetUserById(managerId);
                Warehouse? warehouse = await _adminRepository.GetWarehouseById(warehouseId);

                // check  if manager and warehouse exists in database
                if (user is null || warehouse is null)
                {
                    return 0;
                }
                return await _adminRepository.AssignManagerToWarehouse(managerId, warehouseId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while assigning manager to warehouse" + ex.Message);
                return -1;
            }
        }

        // Get All Warehouses
        public async Task<List<WarehouseDTO>?> GetAllWarehousesAsync()
        {
            try
            {
                List<Warehouse>? warehouses = await _adminRepository.GetAllWarehouses();

                return _mapper.Map<List<WarehouseDTO>>(warehouses);
               

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while getting all warehouses" + ex.Message);
                return null;
            }
        }

        // Get Warehouse By Id
        public async Task<Warehouse?> GetWarehouseByIdAsync(int warehouseId)
        {
            try
            {
                if (warehouseId <= 0)
                {
                    return null;
                }
                return await _adminRepository.GetWarehouseById(warehouseId);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while getting warehouse " + ex.Message);
                return null;
            }
        }

        // Add Warehouse
        public async Task<int> AddWarehouseAsync(Warehouse warehouse)
        {
            try
            {
                if (warehouse == null)
                    return -1;

                int addedWarehouseId = await _adminRepository.AddWarehouse(warehouse);

                return addedWarehouseId;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while adding warehouse" + ex.Message);
                return -1;
            }
        }

        // Update Warehouse
        public async Task<int> UpdateWarehouseAsync(Warehouse warehouse)
        {
            try
            {
                if (warehouse == null)
                    return -1;

                int updatedWarehouseId = await _adminRepository.UpdateWarehouse(warehouse);

                return updatedWarehouseId;


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while updating warehouse" + ex.Message);
                return -1;
            }
        }

        // Remove Warehouse
        public async Task<int> RemoveWarehouseAsync(int warehouseId)
        {
            try
            {
                if (warehouseId <= 0)
                {
                    return -1;
                }
                int deletedWarehouseId = await _adminRepository.RemoveWarehouse(warehouseId);
                return deletedWarehouseId;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while removing warehouse" + ex.Message);
                return -1;
            }
        }


        // Get Admin Summary Statistics 
        public async Task<AdminSummaryStatisticsDTO?> GetAdminSummaryStatisticsAsync()
        {
            try
            {
                int warehouseCount = await _adminRepository.GetTotalWarehousesCount();
                int managerCount = await _adminRepository.GetTotalUsersCount((int)UserRoles.Manager);
                int driverCount = await _adminRepository.GetTotalUsersCount((int)UserRoles.Driver);
                int customerCount = await _adminRepository.GetTotalUsersCount((int)UserRoles.Customer);
                int totalOrdersCount= await _adminRepository.GetTotalOrdersCount();
                decimal totalSales= await _adminRepository.GetTotalSalesAmount();

                return new AdminSummaryStatisticsDTO
                {
                    WarehousesCount = warehouseCount,
                    ManagersCount = managerCount,
                    DriversCount = driverCount,
                    CustomersCount = customerCount,
                    TotalOrdersCount = totalOrdersCount,
                    TotalSalesAmount = totalSales
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while getting admin summary statistics" + ex.Message);
                return null;
            }
        }
    }
}
