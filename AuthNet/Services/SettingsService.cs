using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        public SettingsService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _context = dbContext;
        }
        public async Task<OperationResponse> SaveCompanyInfoAsync(CompanyInfoDto dto, HttpRequest request)
        {
            try
            {
                var exists = await _context.CompanyInfos.AnyAsync();
                if (exists)
                {
                    return new OperationResponse
                    {
                        Success = false,
                        Message = "Company info already exists. Use update instead."
                    };
                }

                var company = new CompanyInfo
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    GSTIN = dto.GSTIN,
                    ContactNumber = dto.ContactNumber,
                    Email = dto.Email
                };

                string imageFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Images");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                // Upload Logo
                if (dto.LogoUrl != null && dto.LogoUrl.Length > 0)
                {
                    var ext = Path.GetExtension(dto.LogoUrl.FileName);
                    var fileName = $"logo_{Guid.NewGuid():N}";
                    var localPath = Path.Combine(imageFolder, $"{fileName}{ext}");

                    using (var stream = new FileStream(localPath, FileMode.Create))
                    {
                        await dto.LogoUrl.CopyToAsync(stream);
                    }

                    var urlPath = $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{fileName}{ext}";

                    company.LogoName = dto.LogoUrl.FileName;
                    company.LogoFileExtension = ext;
                    company.LogoFileSizeInBytes = dto.LogoUrl.Length;
                    company.LogoFilePath = urlPath;
                }

                // Upload Signature
                if (dto.SignatureUrl != null && dto.SignatureUrl.Length > 0)
                {
                    var ext = Path.GetExtension(dto.SignatureUrl.FileName);
                    var fileName = $"sign_{Guid.NewGuid():N}";
                    var localPath = Path.Combine(imageFolder, $"{fileName}{ext}");

                    using (var stream = new FileStream(localPath, FileMode.Create))
                    {
                        await dto.SignatureUrl.CopyToAsync(stream);
                    }

                    var urlPath = $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{fileName}{ext}";

                    company.SignName = dto.SignatureUrl.FileName;
                    company.SignFileExtension = ext;
                    company.SignFileSizeInBytes = dto.SignatureUrl.Length;
                    company.SignFilePath = urlPath;
                }

                _context.CompanyInfos.Add(company);
                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true,
                    Message = "Company info saved successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");

                return new OperationResponse
                {
                    Success = false,
                    Message = "An error occurred while saving company info."
                };
            }
        }

        public async Task<CompanyInfoResponseDto> GetCompanyInfoAsync()
        {
            var company = await _context.CompanyInfos.FirstOrDefaultAsync();

            if (company == null)
                return null;

            return new CompanyInfoResponseDto
            {
                Name = company.Name,
                Address = company.Address,
                GSTIN = company.GSTIN,
                ContactNumber = company.ContactNumber,
                Email = company.Email,
                LogoUrl = company.LogoFilePath,
                SignatureUrl = company.SignFilePath
            };
        }

        public async Task<List<CompanyInfoHistoryDto>> GetCompanyHistoryInfoAsync()
        {
            var history = await _context.CompanyInfoHistories
                .OrderByDescending(h => h.ArchivedAt)
                .ToListAsync();

            return history.Select(h => new CompanyInfoHistoryDto
            {
                Id = h.Id,
                ArchivedAt = h.ArchivedAt,
                Name = h.Name,
                Address = h.Address,
                ContactNumber = h.ContactNumber,
                Email = h.Email,
                GSTIN = h.GSTIN
            }).ToList();
        }


        public async Task<OperationResponse> UpdateCompanyInfoAsync(CompanyInfoDto dto, HttpRequest request)
        {
            try
            {
                var company = await _context.CompanyInfos.FirstOrDefaultAsync();
                if (company == null)
                {
                    return new OperationResponse { Success = false, Message = "Company info not found." };
                }

                var history = new CompanyInfoHistory
                {
                    Name = company.Name,
                    Address = company.Address,
                    GSTIN = company.GSTIN,
                    ContactNumber = company.ContactNumber,
                    Email = company.Email,
                    ArchivedAt = DateTime.UtcNow
                };
                await _context.CompanyInfoHistories.AddAsync(history);

                company.Name = dto.Name;
                company.Address = dto.Address;
                company.GSTIN = dto.GSTIN;
                company.ContactNumber = dto.ContactNumber;
                company.Email = dto.Email;

                string imageFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Images");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                // Logo upload
                if (dto.LogoUrl != null && dto.LogoUrl.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(company.LogoFilePath))
                    {
                        var oldPath = Path.Combine(imageFolder, Path.GetFileName(company.LogoFilePath));
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }

                    var ext = Path.GetExtension(dto.LogoUrl.FileName).ToLower();
                    var fileName = $"logo_{Guid.NewGuid():N}{ext}";
                    var localPath = Path.Combine(imageFolder, fileName);

                    using (var stream = new FileStream(localPath, FileMode.Create))
                        await dto.LogoUrl.CopyToAsync(stream);

                    company.LogoName = dto.LogoUrl.FileName;
                    company.LogoFileExtension = ext;
                    company.LogoFileSizeInBytes = dto.LogoUrl.Length;
                    company.LogoFilePath = $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{fileName}";
                }

                // Signature upload
                if (dto.SignatureUrl != null && dto.SignatureUrl.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(company.SignFilePath))
                    {
                        var oldPath = Path.Combine(imageFolder, Path.GetFileName(company.SignFilePath));
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }

                    var ext = Path.GetExtension(dto.SignatureUrl.FileName).ToLower();
                    var fileName = $"sign_{Guid.NewGuid():N}{ext}";
                    var localPath = Path.Combine(imageFolder, fileName);

                    using (var stream = new FileStream(localPath, FileMode.Create))
                        await dto.SignatureUrl.CopyToAsync(stream);

                    company.SignName = dto.SignatureUrl.FileName;
                    company.SignFileExtension = ext;
                    company.SignFileSizeInBytes = dto.SignatureUrl.Length;
                    company.SignFilePath = $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{fileName}";
                }

                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true,
                    Message = "Company info updated successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateCompanyInfoAsync] {ex.Message}");
                return new OperationResponse
                {
                    Success = false,
                    Message = "An error occurred while updating company info."
                };
            }
        }



        public async Task<bool> SoftResetCompanyInfoAsync()
        {
            var company = await _context.CompanyInfos.FirstOrDefaultAsync();
            if (company == null)
                return false;

            string imageFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Images");

            // Delete logo
            if (!string.IsNullOrEmpty(company.LogoFilePath))
            {
                var logoPath = Path.Combine(imageFolder, Path.GetFileName(company.LogoFilePath));
                if (File.Exists(logoPath)) File.Delete(logoPath);
            }

            // Delete signature
            if (!string.IsNullOrEmpty(company.SignFilePath))
            {
                var signPath = Path.Combine(imageFolder, Path.GetFileName(company.SignFilePath));
                if (File.Exists(signPath)) File.Delete(signPath);
            }

            // Soft reset fields
            company.Name = string.Empty;
            company.Address = string.Empty;
            company.GSTIN = string.Empty;
            company.ContactNumber = string.Empty;

            company.LogoName = null;
            company.LogoFilePath = null;
            company.LogoFileExtension = null;
            company.LogoFileSizeInBytes = 0;

            company.SignName = null;
            company.SignFilePath = null;
            company.SignFileExtension = null;
            company.SignFileSizeInBytes = 0;

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
