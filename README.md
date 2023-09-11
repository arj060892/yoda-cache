
# Yoda In-memory Cache Solution

## Overview
Yoda is an application designed to handle large volumes of data efficiently. The heart of this efficiency is a generic in-memory cache component. This cache, while simple in its operation, is architected using SOLID principles to ensure scalability, maintainability, and extensibility.

## SOLID Principles in Action
1. **Single Responsibility Principle (SRP)**: Each class and module focuses on a single responsibility. For instance, eviction strategies are decoupled from the storage mechanism.
2. **Open-Closed Principle (OCP)**: The design uses interfaces extensively, allowing us to introduce new behaviors without altering existing code. You can introduce a new eviction strategy or storage backend with ease.
3. **Liskov Substitution Principle (LSP)**: The cache can be replaced with any other implementation of the `ICache` interface without affecting its consumers.
4. **Interface Segregation Principle (ISP)**: The system uses fine-grained interfaces like `IEvictionStrategy` and `IStorageBackend`, ensuring that implementing classes aren't burdened with unnecessary methods.
5. **Dependency Inversion Principle (DIP)**: High-level modules (like the cache operations) are not dependent on low-level modules but depend on abstractions. This makes the system flexible and easier to refactor.

## Extensibility
The architecture is consciously designed for extensibility:
- **Eviction Strategies**: Implement a new class adhering to the `IEvictionStrategy` interface to bring in different eviction policies.
- **Storage Backends**: The cache isn't tied to a particular storage mechanism. By creating a new class that implements the `IStorageBackend` interface, it's straightforward to introduce new storage methods.
- **API Integration**: The cache component is easily pluggable into any API or service, as demonstrated with the `UsersController`.

## Features
- **Generic Cache**: The cache component is type-safe, making it adaptable to various data types.
- **LRU Eviction Strategy**: This strategy ensures optimal cache utilization by evicting the least recently used item.
- **Thread-Safe**: Critical sections are guarded using locks, ensuring thread safety.
- **Event-Driven Eviction Notification**: Subscribers can be notified when an item is evicted, enhancing the interactive capability of the cache.

## Getting Started
### Setting Up
1. Clone the repository.
2. Restore the required packages.
3. Build the solution.

### Running
1. Run the API project.
2. Access the API endpoints using tools like Postman or via Swagger.

## Tests
Unit tests, created using NUnit, Moq, and AutoFixture, are provided to ensure robustness and reliability.

## License
[MIT](https://choosealicense.com/licenses/mit/)
