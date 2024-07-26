using System.Collections.Generic;

namespace SharedKernel;

public abstract class BaseEntity
{
	public List<BaseDomainEvent> Events = new();
}
