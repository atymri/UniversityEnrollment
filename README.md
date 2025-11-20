# UniversityEnrollment

UniversityEnrollment is a C# project designed to streamline student enrollment and course management for academic institutions. A key focus of this project is robust error handling using the **Result Pattern**, providing clear and structured results from operations rather than relying on exceptions.

## Features

- Student enrollment, registration, and course management
- Separation of concerns (API, core logic, infrastructure, and tests)
- **Result Pattern** for reliable error handling and communication across layers
- Extensible architecture for educational domain systems

## Result Pattern

The project uses the Result Pattern to handle errors and communicate outcomes:
- Success and failure results are consistently returned from methods.
- Reduces exception-based flow and improves code readability.
- Makes it easier to compose operations and propagate errors.

This pattern is implemented across the core logic, API, and infrastructure layers. Relevant components allow you to quickly identify, handle, and act on operational success or failure.

## Project Structure

- `UniversityEnrollment.API` – API endpoints for enrollment.
- `UniversityEnrollment.Core` – Business logic using the Result Pattern.
- `UniversityEnrollment.Infrastructure` – Data and service integration, error propagation with Result objects.
- `UniversityEnrollment.Tests` – Unit and integration tests focusing on result scenarios.

## Getting Started

1. **Clone the repository**
    ```bash
    git clone https://github.com/atymri/UniversityEnrollment.git
    ```
2. **Open in Visual Studio**
    - Load the solution with `UniversityEnrollment.slnx`.

3. **Build and Run**
    - NuGet dependencies restore automatically.
    - Build and start the API project.

## License
Licensed under the MIT License. See `LICENSE.txt` for details.
