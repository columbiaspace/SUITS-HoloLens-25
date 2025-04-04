from pathfinding.core.grid import Grid
from pathfinding.finder.a_star import AStarFinder
from pathfinding.core.diagonal_movement import DiagonalMovement

# Create a 3x3 grid with no obstacles
# Create a simple 3x3 grid
# Create a larger 8x8 grid with obstacles forming a maze-like pattern
# Create a 27x28 grid initialized with all walkable paths (1s)
matrix = [[1 for _ in range(28)] for _ in range(27)]

# Set obstacles (0s) at specified positions
obstacles = [
    (22, 12), (22, 11), (22, 10),
    (23, 10), (23, 11), (23, 10),
    (24, 12), (24, 11),
    (27, 10), (27, 9), (27, 7),
    (27, 6), (27, 5),
    (26, 5), (25, 5),
    (25, 6), (26, 6), (26, 7)
]

# Place obstacles in the matrix
for x, y in obstacles:
    if x < len(matrix[0]) and y < len(matrix):
        matrix[y][x] = 0

# Create grid from matrix
grid = Grid(matrix=matrix)

# Define start and end positions
start_x, start_y = 0, 0  # top-left
end_x, end_y = 7, 7      # bottom-right

start = grid.node(start_x, start_y)
end = grid.node(end_x, end_y)

# Configure pathfinder
finder = AStarFinder(
    diagonal_movement=DiagonalMovement.always,
    time_limit=10
)

# Find path
path, runs = finder.find_path(start, end, grid)

# Print debug information
print('Grid Configuration:')
for row in matrix:
    print(row)
print(f'\nStart: ({start_x},{start_y})')
print(f'End: ({end_x},{end_y})')
print(f'Path found: {[(node.x, node.y) for node in path]}')
print(f'Operations run: {runs}')

# Visualize the path
if path:
    print('\nPath visualization:')
    for y in range(len(matrix)):
        for x in range(len(matrix[y])):
            if any(node.x == x and node.y == y for node in path):
                print('X', end=' ')  # path cells
            elif matrix[y][x] == 0:
                print('O', end=' ')  # obstacles
            else:
                print('•', end=' ')  # empty spaces
        print()
else:
    print('\nNo path found!')