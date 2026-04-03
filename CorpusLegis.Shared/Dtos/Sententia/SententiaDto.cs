using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Sententia;

public record SententiaDto(
    Guid Id,
    Guid? ParentId,
    Guid CivisId,
    string CivisName,
    string Content,
    DateTime CreatedAt,
    bool IsEdited,
    bool IsDeleted,
    bool CanDelete,
    //bool CanEdit,
    //bool CanRestore,
    List<SententiaDto> Replies
);
