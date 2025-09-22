using ECommerce.Domain.Entities;
using System;

namespace ECommerce.Domain.Common;

/// <summary>
/// Một interface đánh dấu (marker interface) cho tất cả các entity
/// sử dụng Guid làm kiểu dữ liệu cho khóa chính (Primary Key).
/// Việc này giúp dễ dàng áp dụng các cấu hình chung trong DbContext.
/// </summary>
public interface IEntityWithGuidId
{
    /// <summary>
    /// Khóa chính của Entity.
    /// </summary>
    Guid Id { get; set; }
}