using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyTodo.WebServer.Data;
using MyTodo.WebServer.DTOs.Memo;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer.Repositories
{
    public class MemoRepository : IMemoRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public MemoRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Memo?> CreateAsync(MemoForCreateDTO MemoForCreateDTO)
        {
            Memo memo = _mapper.Map<Memo>(MemoForCreateDTO);
            DateTime now = DateTime.Now;
            memo.CreatedAt = now;
            memo.UpdatedAt = now;
            await _dbContext.Memos.AddAsync(memo);
            int result = await _dbContext.SaveChangesAsync();
            if (result == 0)
                return null;
            return memo;
        }

        public async Task<Memo?> DeleteAsync(int id)
        {
            Memo? memo = await _dbContext.Memos.FindAsync(id);
            if (memo == null)
                return null;
            _dbContext.Memos.Remove(memo);
            await _dbContext.SaveChangesAsync();
            return memo;
        }

        public async Task<List<Memo>> GetAllAsync(string? title = null)
        {
            var memoQueryable = _dbContext.Memos.AsQueryable();

            if (title != null && !string.IsNullOrWhiteSpace(title))
            {
                memoQueryable = memoQueryable.Where(m => m.Title.Contains(title));
            }

            List<Memo> memos = await memoQueryable
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return memos;
        }

        public async Task<Memo?> GetByIdAsync(int id)
        {
            var memo = await _dbContext.Memos.FindAsync(id);
            if (memo == null)
                return null;
            return memo;
        }

        public async Task<Memo?> UpdateAsync(int id, MemoForUpdateDTO MemoForUpdateDTO)
        {
            var memo = await _dbContext.Memos.FindAsync(id);
            if (memo == null)
                return null;

            memo.Title = MemoForUpdateDTO.Title;
            memo.Content = MemoForUpdateDTO.Content;
            memo.UpdatedAt = DateTime.Now;
            int result = await _dbContext.SaveChangesAsync();
            if (result == 0)
                return null;
            return memo;
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Memos.CountAsync();
        }
    }
}
